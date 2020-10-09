using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.InternalModels;
using HotlineKatalog.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Services
{
    public class ComfyParseService : IComfyParseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random;
        private readonly IAddDBService _addDBService;

        public ComfyParseService(IUnitOfWork unitOfWork, IAddDBService addDBService)
        {
            _unitOfWork = unitOfWork;
            _addDBService = addDBService;
            _random = new Random();
        }

        public async Task Parse()
        {
            var brovser = await StartBrovser();

            var shop = _unitOfWork.Repository<Shop>().Get(x => x.Name == "Comfy")
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .Include(x => x.Tags)
                                                        .FirstOrDefault();

            List<string> pageLinks = null;

            foreach (var category in shop.Categories)
            {
                var doing = true;
                var pageUrl = category.Url;

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
                            var itemHtml = await GetHtml(brovser, itemLink);

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
                        pageUrl = await GetNextPageLink(page, shop.Tags.NextPageTag);
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

        public async Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodUrlTag)                                    // get page items with class "{goodUrlTag}"
                                                                        && !x.HasClass("_next")                                     // w/o "_next" => "завантажити ще 20 товарыв"
                                                                        && !x.Descendants().Any(y => y.HasClass("js-more-button"))  // w/o "js-more-button" => "Детальныше" (cant buy)
                                                                        && !x.HasClass("_empty")                                    // w/o "_empty" => empty blocks
                                                                        && !x.HasClass("news-item")                                 // w/o "news-item" => news
                                                                        && !x.HasClass("products-list__item_banner")                // w/o "news-item" => news
                                                                    ).ToList();

            return bodyTags;
        }

        public async Task<string> GetNextPageLink(string html, string nextPageTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var nextPage = document.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass(nextPageTag));

            var link = nextPage.Descendants().FirstOrDefault(f => f.Attributes.Any(a => a.Name == "href")).GetAttributeValue("href", string.Empty);

            return link;
        }

        public async Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag)
        {
            // select from nodes
            // all nodes with class {goodUrlTag}
            // take from every node attribute href and it value
            var links = bodyTags.Select(x => x.Descendants().Where(d => d.HasClass(goodUrlTag))
                                                .FirstOrDefault(f => f.Attributes.Any(a => a.Name == "href"))
                                                .GetAttributeValue("href", string.Empty))
                                .ToList();

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

        public async Task<string> GetHtml(Browser brovser, string link)
        {
            var page = await brovser.NewPageAsync();
            Thread.Sleep(_random.Next(3000, 5000));
            await page.GoToAsync(link);
            Thread.Sleep(_random.Next(5000, 10000));
            var html = await page.GetContentAsync();
            return html;
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

        public async Task<int> GetPrice(IEnumerable<HtmlNode> bodyTags, string priceTag)
        {
            // get attribute "content" with price
            var price = int.Parse(bodyTags.FirstOrDefault(d => d.HasClass(priceTag) && d.Attributes.Any(a => a.Name == "content"))
                                            .GetAttributeValue("content", string.Empty));

            return price;
        }

        public async Task<Dictionary<string, string>> GetSpecification(IEnumerable<HtmlNode> bodyTags, string specificationTag)
        {
            var list = bodyTags.Where(d => d.HasClass(specificationTag));

            var response = new Dictionary<string, string>();

            foreach (var item in list)
            {
                var key = item.Descendants().FirstOrDefault(x => x.HasClass("title")).InnerText.Trim();
                var value = item.Descendants().FirstOrDefault(x => x.HasClass("value")).InnerText.Trim();

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



        private async Task<string> GetHtml(string link)
        {
            WebResponse response = null;
            StreamReader reader = null;

            string html = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                request.Method = "GET";
                request.ContentType = "text/html;charset=ISO-8859-1";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
                response = await request.GetResponseAsync();
                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                html = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            return html;
        }
    }
}

