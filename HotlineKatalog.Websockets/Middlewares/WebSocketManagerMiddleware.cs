using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.WebSockets.Handlers;
using HotlineKatalog.WebSockets.Interfaces;
using HotlineKatalog.WebSockets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HotlineKatalog.WebSockets.Middlewares
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketMessageHandler _webSocketHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebSocketManagerMiddleware> _logger;

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketMessageHandler webSocketHandler, IHttpContextAccessor httpContextAccessor, ILogger<WebSocketManagerMiddleware> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
            _webSocketHandler = webSocketHandler;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork/*, HashUtility hashService*/)
        {
            _logger.LogTrace("WebSocketManagerMiddleware.Invoke(HttpContext) called");

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await context.Response.WriteAsync("Web socket");
                return;
            }

            StringValues authorizationToken = "";

            bool isBrowserRequest = false;

            _logger.LogInformation("WebSocketManagerMiddleware.Invoke(HttpContext) - validate access token started");

            //var tokenReceived = context.Request.Headers.TryGetValue("Authorization", out authorizationToken);

            //if (!tokenReceived)
            //{
            //    tokenReceived = context.Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out authorizationToken);
            //    isBrowserRequest = true;
            //}

            //if (tokenReceived)
            //{
            //var accessTokenHash = hashService.GetHash(authorizationToken);

            WebSocket socket = null;

            _logger.LogInformation("WebSocketManagerMiddleware.Invoke(HttpContext) - request to DB");

            bool isTestConnection = authorizationToken == "test";

            if (!isTestConnection)
            {
                try

                {
                    var now = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError("WebSocketManagerMiddleware.Invoke => Exception occured in DB");
                    return;
                }

                _logger.LogInformation("WebSocketManagerMiddleware.Invoke(HttpContext) - request to DB finished");
            }

            if (isBrowserRequest)
                socket = await context.WebSockets.AcceptWebSocketAsync(authorizationToken);
            else
                socket = await context.WebSockets.AcceptWebSocketAsync();

            IWSItem item = null;

            try
            {
                item = await _webSocketHandler.Connect(socket, new WSData
                {
                    //TokenId = token.Id,
                    //UserId = token.UserId,
                    //Roles = token.User.UserRoles.Select(w => w.Role.Name).ToArray()
                });

                _logger.LogInformation("WebSocketManagerMiddleware.Invoke(HttpContext) - socket with id {" + item.TokenId + "} connected");

                if (socket.State == WebSocketState.Open || socket.State == WebSocketState.Connecting)
                {
                    _logger.LogInformation("WebSocketManagerMiddleware.Invoke(HttpContext) - init socket handlers");

                    await Receive(socket, async (result, buffer) =>
                    {
                        var currSid = _webSocketHandler.GetTokenId(socket);
                        if (currSid != null)
                        {
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                try
                                {
                                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Exception occured in WebSocketManagerMiddleware.WebSocketHandler.ReceiveAsync; Sid = " + currSid + "; Exception.Message: " + ex.Message);
                                }

                                return;
                            }
                            else if (result.MessageType == WebSocketMessageType.Close)
                            {
                                try
                                {
                                    await _webSocketHandler.Disconnect(socket);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Exception occured in WebSocketManagerMiddleware.WebSocketHandler.OnDisconnected; Sid = " + currSid + ";  Exception.Message: " + ex.Message);
                                }
                                return;
                            }
                        }
                    });
                }
                else
                {
                    await _webSocketHandler.RemoveWebSocketConnection(socket);
                    _logger.LogError("Exception occured in WebSocketManagerMiddleware.Invoke(HttpContext) - websocket with id {" + item?.TokenId + "} disconnected");
                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("without completing the close handshake"))
                {
                    _logger.LogError("Exception occured in WebSocketManagerMiddleware.Invoke -> Receive; tId = " + item?.TokenId + "; Exception.Message: " + ex.Message + ". WebSocket will be removed");
                    await _webSocketHandler.RemoveWebSocketConnection(socket);
                }
                else
                {
                    _logger.LogError("Exception occured in WebSocketManagerMiddleware.Invoke -> Receive; Sid = " + item?.TokenId + "; Exception.Message: " + ex.Message);
                }
                return;
            }
            //}
            //else
            //{
            //    _logger.LogError("WebSocketManagerMiddleware.Invoke(HttpContext) - access token is empty");
            //    return;
            //}

            _logger.LogTrace("WebSocketManagerMiddleware.Invoke(HttpContext) request finished");
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            _logger.LogTrace("WebSocketManagerMiddleware.Receive(WebSocket, Action<WebSocketReceiveResult, byte[]>) started");

            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
            WebSocketReceiveResult result = null;

            while (socket.State == WebSocketState.Open || socket.State == WebSocketState.Connecting)
            {
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    var bufferedData = ms.ToArray();

                    handleMessage(result, bufferedData);
                }
            }

            try
            {
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                var tokenId = _webSocketHandler.GetTokenId(socket);

                if (tokenId != null)
                {
                    await _webSocketHandler.RemoveWebSocketConnection(socket);
                }

                if (_webSocketHandler.GetTokenId(socket) == null)
                {
                    _logger.LogInformation("WebSocketConnectionManager.Receive -> All connections with id {" + tokenId + "} have been closed.");
                }
            }
            catch (WebSocketException ex)
            {
                var tokenId = _webSocketHandler.GetTokenId(socket);

                _logger.LogError("WebSocketConnectionManager.Receive -> Exception occured during connection closing. Connection id: " + tokenId + ". Exception message: " + ex.Message + ". WebSocket state - " + Enum.GetName(typeof(WebSocketState), socket.State));

                if (tokenId != null)
                    await _webSocketHandler.RemoveWebSocketConnection(socket);

                if (_webSocketHandler.GetTokenId(socket) == null)
                {
                    _logger.LogInformation("WebSocketConnectionManager.Receive -> All connections with id {" + tokenId + "} have been closed.");
                }

                throw new ArgumentException("WebSocketConnectionManager.Receive -> Exception occured during connection closing. Connection id: " + tokenId + ". Exception message: " + ex.Message);
            }

            _logger.LogTrace("WebSocketManagerMiddleware.Receive(WebSocket, Action<WebSocketReceiveResult, byte[]>) ended");
        }
    }
}
