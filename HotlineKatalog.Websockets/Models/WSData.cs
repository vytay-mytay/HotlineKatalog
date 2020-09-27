using HotlineKatalog.WebSockets.Interfaces;

namespace HotlineKatalog.WebSockets.Models
{
    public class WSData: IWSItem
    {
        public int TokenId { get; set; }

        public int UserId { get; set; }

        public string[] Roles { get; set; } 
    }
}
