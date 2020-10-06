using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Interfaces
{
    public abstract class AbstractParse : IParseService
    {
        protected readonly Random _random;

        public AbstractParse()
        {
            _random = new Random();
        }

        public async Task<string> GetHtml(Browser brovser, string link)
        {
            var page = await brovser.NewPageAsync();
            Thread.Sleep(_random.Next(3000, 5000));
            await page.GoToAsync(link);
            Thread.Sleep(_random.Next(5000, 10000));
            var html = await page.GetContentAsync();
            return html;
        }

        public async Task<string> GetHtml(string curl)
        {
            var html = File.ReadAllText(curl);

            return html;
        }

        public virtual Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag)
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName)
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> GetNextPageLink(string html, string nextPageTag)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> GetPrice(IEnumerable<HtmlNode> bodyTags, string priceTag)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Dictionary<string, string>> GetSpecification(IEnumerable<HtmlNode> bodyTags, string specificationTag)
        {
            throw new NotImplementedException();
        }

        public virtual Task Parse()
        {
            throw new NotImplementedException();
        }

        public async Task<Browser> StartBrovser()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false
            });

            return browser;
        }
    }
}
