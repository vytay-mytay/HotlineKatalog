using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.InternalModels;
using HotlineKatalog.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Services
{
    public class EldoradoParseService : IEldoradoParseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddDBService _addDBService;
        private readonly Random _random;

        public EldoradoParseService(IUnitOfWork unitOfWork, IAddDBService addDBService)
        {
            _unitOfWork = unitOfWork;
            _addDBService = addDBService;
            _random = new Random();
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

        public async Task<string> GetHtml(Browser brovser, string link)
        {
            var page = await brovser.NewPageAsync();
            Thread.Sleep(_random.Next(3000, 5000));
            await page.GoToAsync(link, timeout: 60000);
            Thread.Sleep(_random.Next(5000, 10000));
            var html = await page.GetContentAsync();
            return html;
        }

        public async Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag)
        {
            var links = bodyTags.Select(d => d.Descendants()
                                            .FirstOrDefault(c => c.HasClass(goodUrlTag))
                                            .ChildNodes
                                            .FirstOrDefault(c => c.Name == "a")
                                            .GetAttributeValue("href", string.Empty)).ToList();

            return links;
        }

        public async Task<string> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName)
        {
            // select from nodes
            // all nodel with class {nameTag}
            // take from every node InetText and replace {categoryName} and trim
            var name = bodyTags.FirstOrDefault(x => x.HasClass(nameTag)).InnerText.Replace(categoryName, "").Replace("вбудовуваний", "").Trim().ToLower();

            return name;
        }

        public async Task<string> GetNextPageLink(string html, string nextPageTag, int i)
        {
            // click on div with "to-left"

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var link = "";

            if (document.DocumentNode.Descendants().Any(x => x.HasClass("to-left")))
            {
                var nextPage = document.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass(nextPageTag) && !x.HasClass("active") && x.InnerText == i.ToString());

                link = nextPage.Descendants().FirstOrDefault(f => f.Attributes.Any(a => a.Name == "href")).GetAttributeValue("href", string.Empty);
            }

            return link;
        }

        public async Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodUrlTag)).ToList();

            return bodyTags;
        }

        public async Task<int> GetPrice(IEnumerable<HtmlNode> bodyTags, string priceTag)
        {
            // get attribute "content" with price
            var price = int.Parse(bodyTags.FirstOrDefault(d => d.HasClass(priceTag) && d.Attributes.Any(a => a.Name == "content"))
                                            .GetAttributeValue("content", string.Empty));

            return price;
        }

        public async Task<Dictionary<string, string>> GetSpecification(IEnumerable<HtmlNode> bodyTags, string specificationTag)
        {
            var specificationNodes = bodyTags.SelectMany(b => b.ChildNodes
                                                                .Where(f => f.HasClass(specificationTag)))
                                                                .SelectMany(t => t.ChildNodes
                                                                                    .Where(c => c != null && c.Name == "li" && c.ChildNodes.All(a => a.Name == "span")));

            var response = new Dictionary<string, string>();

            foreach (var item in specificationNodes)
            {
                var key = item.ChildNodes.FirstOrDefault(x => !x.HasClass("attribute-val")).InnerText.Trim();
                var value = item.ChildNodes.FirstOrDefault(x => x.HasClass("attribute-val")).InnerText.Trim();

                var canNotAdd = false;

                if (response.Any(x => x.Key == key))
                    canNotAdd = true;

                while (canNotAdd)
                {
                    key += '1';
                    if (!response.Any(x => x.Key == key))
                        canNotAdd = false;
                }

                response.Add(key, value);
            }

            return response;
        }

        public async Task Parse()
        {
            var brovser = await StartBrovser();

            var shop = _unitOfWork.Repository<Shop>().Get(x => x.Name == "Eldorado")
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .Include(x => x.Tags)
                                                        .FirstOrDefault();

            List<string> pageLinks = null;

            foreach (var category in shop.Categories)
            {
                var doing = true;
                var pageUrl = category.Url;
                //var pageUrl = "https://eldorado.ua/uk/holodilniki/c1061560/page=25/";

                int i = 1;

                while (doing)
                {
                    string page = await GetHtml(brovser, pageUrl);

                    if (page.PadLeft(100).Contains("NOINDEX, NOFOLLOW"))
                        break;

                    var bodyTags = await GetPageItems(page, shop.Tags.PageItemTag);

                    if (bodyTags.Any())
                    {
                        var item = new PriceInternalModel()
                        {
                            Category = category.Category,
                            Shop = shop
                        };

                        pageLinks = await GetLinks(bodyTags, shop.Tags.GoodUrlTag);

                        foreach (var itemLink in pageLinks)
                        {
                            var link = itemLink;
                            if (!itemLink.StartsWith(shop.Url))
                                link = itemLink.Insert(0, shop.Url);

                            var itemHtml = await GetHtml(brovser, link);

                            var document = new HtmlDocument();
                            document.LoadHtml(itemHtml);

                            if (itemHtml.PadLeft(100).Contains("NOINDEX, NOFOLLOW"))
                                continue;

                            item.Url = itemLink;
                            // get name
                            item.Name = await GetName(document.DocumentNode.Descendants(), shop.Tags.NameTag, category.Category.Name);
                            item.ProducerName = item.Name.Split(new char[] { ' ' })[0];
                            // get price
                            item.Price = await GetPrice(document.DocumentNode.Descendants(), shop.Tags.PriceTag);
                            // get specification (characteristics)
                            item.Specification = await GetSpecification(document.DocumentNode.Descendants(), shop.Tags.SpecificationTag);

                            var good = await _addDBService.AddToDB(item);
                        }

                        i++;

                        pageUrl = await GetNextPageLink(page, shop.Tags.NextPageTag, i);

                        if (pageUrl == null || pageUrl.Any())
                            doing = false;
                    }
                    else
                        doing = false;

                    var pages = await brovser.PagesAsync();
                    foreach (var item in pages.Take(pages.Length - 1))
                    {
                        await item.CloseAsync();
                    }
                }
            }
            await brovser.CloseAsync();
        }
    }
}
