using HotlineKatalog.WebSockets.Interfaces;
using System.Net.WebSockets;

namespace HotlineKatalog.WebSockets.Models
{
    public class WSItem<ConnectedData> where ConnectedData : class, IWSItem
    {
        public WebSocket Socket { get; set; }

        public ConnectedData Data { get; set; }
    }
}
