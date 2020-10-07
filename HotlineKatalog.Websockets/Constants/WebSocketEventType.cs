namespace HotlineKatalog.WebSockets.Constants
{
    public class WebSocketEventType
    {
        public const string Ping = "ping";

        public const string Pong = "pong";

        public const string Typing = "typing";

        public const string Message = "message";

        public const string NewNotification = "new_notification";

        public const string NewMessage = "chat_new_message";

        public const string WagerUnreadMessagesUpdated = "chat_wager_unread_updated";

        public const string TotalUnreadMessagesUpdated = "chat_total_unread_updated";

        public const string MessageRead = "chat_message_read";

        public const string OnlineStatusChanged = "online_status_changed";

        public const string UpdatedNotification = "updated_notification";

        public const string RemovedNotification = "removed_notification";

        public const string PriceChange = "price_change";
    }
}
