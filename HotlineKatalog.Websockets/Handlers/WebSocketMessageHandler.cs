using HotlineKatalog.WebSockets.Abstract;
using HotlineKatalog.WebSockets.Constants;
using HotlineKatalog.WebSockets.Interfaces;
using HotlineKatalog.WebSockets.Managers;
using HotlineKatalog.WebSockets.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HotlineKatalog.WebSockets.Handlers
{
    public class WebSocketMessageHandler : WebSocketHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebSocketMessageHandler> _log;
        private readonly IServiceProvider _serviceProvider;

        private HashSet<int> _userIdsConnections;

        public WebSocketMessageHandler(WebSocketConnectionManager<IWSItem> webSocketConnectionManager, ILogger<WebSocketMessageHandler> log, IConfiguration configuration, IServiceProvider serviceProvider)
            : base(webSocketConnectionManager, log)
        {
            _configuration = configuration;
            _log = log;
            _serviceProvider = serviceProvider;

            _userIdsConnections = GetAll().Select(x => x.Data.UserId).Distinct().ToHashSet();
        }

        public async Task SendMessageToUserAsync(int userId, WebSocketEventResponseModel message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);

            var socketsData = WebSocketConnectionManager.Get(w => w.Data.UserId == userId);

            if (socketsData.Count > 0)
            {
                foreach (var socketData in socketsData)
                {
                    if (socketData.Socket.State != WebSocketState.Open)
                    {
                        _log.LogInformation($"WebSocket => socket {socketData.Data.TokenId} is " + Enum.GetName(typeof(WebSocketState), socketData.Socket.State));
                        return;
                    }

                    await SendMessageAsync(socketData.Socket, serializedMessage);
                }
            }
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetTokenId(socket);

            if (socketId != null)
            {
                var socketData = WebSocketConnectionManager.GetSocketDataByTokenId(socketId.Value);

                var json = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                var requestMessage = JsonConvert.DeserializeObject<WebSocketEventResponseModel>(json);

                WebSocketEventResponseModel webSocketResponseMessage = null;
                string responseMessage = null;

                switch (requestMessage?.EventType?.ToLower())
                {
                    case WebSocketEventType.Ping:

                        webSocketResponseMessage = new WebSocketEventResponseModel
                        {
                            EventType = WebSocketEventType.Pong
                        };

                        responseMessage = JsonConvert.SerializeObject(webSocketResponseMessage);

                        await SendMessageAsync(socket, responseMessage);
                        break;
                    case WebSocketEventType.Message:
                        var opponent = WebSocketConnectionManager.Get(w => w.Data.UserId == socketData.UserId).FirstOrDefault();

                        await SendMessageAsync(opponent.Socket, json);

                        break;
                }
            }
        }

        public async Task RemoveWebSocketConnectionByTokenId(int tokenId)
        {
            var webSocket = WebSocketConnectionManager.GetSocketByTokenId(tokenId);
            if (webSocket != null)
            {
                try
                {
                    _log.LogInformation("ChatMessageHandler.RemoveWebSocketConnection -> Close connection for WebSocket with id " + tokenId);
                    await WebSocketConnectionManager.RemoveSocketAsync(webSocket, WebSocketCloseStatus.PolicyViolation, "Invalid connection");
                }
                catch (Exception ex)
                {
                    _log.LogError("ChatMessageHandler.RemoveWebSocketConnection -> Exception occured during connection closing. Connection id: " + tokenId + ". Exception message: " + ex.Message);
                    WebSocketConnectionManager.RemoveSocket(webSocket);
                }
            }
        }

        public override async Task<IWSItem> Connect(WebSocket socket, IWSItem data)
        {
            // close all previous connections
            //await Disconnect(data.UserId, socket);

            var result = await WebSocketConnectionManager.AddSocket(socket, data);

            // add user id for statistics
            if (!_userIdsConnections.Contains(data.UserId))
                _userIdsConnections.Add(data.UserId);

            return result;
        }

        public override async Task Disconnect(WebSocket socket)
        {
            // Get socket
            var socketData = WebSocketConnectionManager.Get(x => x.Socket == socket)
                .FirstOrDefault();

            if (socketData != null)
            {
                try
                {
                    _log.LogInformation("WebSocketHandler.OnDisconnected -> WebSocket with id {" + socketData.Data.TokenId + "} disconected");
                    await WebSocketConnectionManager.RemoveSocketAsync(socket, WebSocketCloseStatus.NormalClosure, "Connection closed");
                }
                catch (Exception ex)
                {
                    _log.LogError("WebSocketHandler.OnDisconnected -> Exception occured during connection closing. Connection id: " + socketData.Data.TokenId + ". Exception message: " + ex.Message);
                    WebSocketConnectionManager.RemoveSocket(socket);
                }
            }
        }

        public async Task Disconnect(int userId)
        {
            var previousConnections = WebSocketConnectionManager.GetSocketsByUserId(userId);
            foreach (var item in previousConnections)
                await Disconnect(item);
        }

        public List<WSItem<IWSItem>> GetAll()
        {
            return WebSocketConnectionManager.GetAll();
        }

        public HashSet<int> GetUserConnections()
        {
            var response = _userIdsConnections.ToHashSet();
            _userIdsConnections = GetAll().Select(x => x.Data.UserId).Distinct().ToHashSet();
            return response;
        }
    }
}
