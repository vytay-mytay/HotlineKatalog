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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    //var comfyParseService = scope.ServiceProvider.GetRequiredService<IComfyParseService>();

                    //await comfyParseService.Parse();

                    //foreach (var shop in shops)
                    //{
                    //    foreach (var category in shop.Categories)
                    //    {
                    //        var doing = true;
                    //        var pageUrl = category.Url;

                    //        while (doing)
                    //        {
                    //            string page = await GetHtml(browser, pageUrl);

                    //            if (page.PadLeft(100).Contains("NOINDEX, NOFOLLOW"))
                    //                break;

                    //            var bodyTags = await GetPageItems(page, shop.Tags.PageItemTag);

                    //            if (bodyTags.Any())
                    //            {
                    //                var item = new PriceInternalModel()
                    //                {
                    //                    Category = category.Category,
                    //                    Shop = shop
                    //                };

                    //                pageLinks = await GetLinks(bodyTags, shop.Tags.GoodUrlTag);

                    //                foreach (var itemLink in pageLinks)
                    //                {
                    //                    var itemHtml = await GetHtml(brovser, itemLink);

                    //                    var document = new HtmlDocument();
                    //                    document.LoadHtml(itemHtml);

                    //                    if (itemHtml.PadLeft(100).Contains("NOINDEX, NOFOLLOW"))
                    //                        continue;

                    //                    item.Url = itemLink;
                    //                    // get name
                    //                    item.Name = await GetName(document.DocumentNode.Descendants(), shop.Tags.NameTag, category.Category.Name);
                    //                    item.ProducerName = item.Name.Split(new char[] { ' ' })[0];
                    //                    // get price
                    //                    item.Price = await GetPrice(document.DocumentNode.Descendants(), shop.Tags.PriceTag);
                    //                    // get specification (characteristics)
                    //                    item.Specification = await GetSpecification(document.DocumentNode.Descendants(), shop.Tags.SpecificationTag);

                    //                    var good = await _addDBService.AddToDB(item);
                    //                }
                    //                pageUrl = await GetNextPageLink(page, shop.Tags.NextPageTag);
                    //            }
                    //            else
                    //                doing = false;

                    //            var pages = await brovser.PagesAsync();
                    //            foreach (var item in pages.Take(pages.Length - 1))
                    //            {
                    //                await item.CloseAsync();
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{LOG_IDENTIFIER} => Exception.Message: {ex.Message}");
            }
        }
    }
}