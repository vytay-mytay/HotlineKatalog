using HotlineKatalog.WebSockets.Handlers;
using HotlineKatalog.WebSockets.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace HotlineKatalog.WebSockets.Extentions
{
    public static class Extentions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient(typeof(WebSocketConnectionManager<>));

            services.AddSingleton(typeof(WebSocketMessageHandler));

            return services;
        }
    }
}
