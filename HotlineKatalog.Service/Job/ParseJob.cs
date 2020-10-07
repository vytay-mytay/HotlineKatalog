using HotlineKatalog.ScheduledTasks;
using HotlineKatalog.ScheduledTasks.Schedule;
using HotlineKatalog.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Job
{
    public class ParseJob : ScheduledTask, IScheduledTask
    {
        private const string LOG_IDENTIFIER = "Parse";

        private ILogger<ParseJob> _logger;
        private readonly IServiceProvider _serviceProvider;


        public ParseJob(IServiceProvider serviceProvider, ILogger<ParseJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logger.LogInformation($"{LOG_IDENTIFIER} => started. At {DateTime.UtcNow.ToShortTimeString()}");

            // every 00:00 
            Schedule = "0 0 */1 * *";

            _nextRunTime = DateTime.UtcNow;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _lastRunTime = _nextRunTime;
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var comfyParseService = scope.ServiceProvider.GetRequiredService<IComfyParseService>();

                    await comfyParseService.Parse();

                    var eldoradoParseService = scope.ServiceProvider.GetRequiredService<IEldoradoParseService>();

                    await eldoradoParseService.Parse();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{LOG_IDENTIFIER} => Exception.Message: {ex.Message}");
            }
        }
    }
}