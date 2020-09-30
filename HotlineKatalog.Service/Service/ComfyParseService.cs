using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Services.Interface;
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

namespace HotlineKatalog.Services.Service
{
    public class ComfyParseService : IParseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random;

        public ComfyParseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _random = new Random();
        }

        public async Task<object> Parse()
        {
            var brovser = await StartBrovser();

            var shop = _unitOfWork.Repository<Shop>().Get(x => x.Name == "Comfy")
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .Include(x => x.Tags)
                                                        .FirstOrDefault();

            List<string> pageLinks = null;
            var names = new List<string>();

            foreach (var category in shop.Categories)
            {
                string page = await GetHtml(brovser, category.Url);

                if (page.Contains("NOINDEX, NOFOLLOW"))
                    continue;

                var bodyTags = await GetPageItems(page, shop.Tags.PageItemTag);
                pageLinks = await GetLinks(bodyTags, shop.Tags.GoodUrlTag);

                foreach (var itemLink in pageLinks)
                {
                    var itemHtml = await GetHtml(brovser, itemLink);

                    var document = new HtmlDocument();
                    document.LoadHtml(itemHtml);

                    if (itemHtml.Contains("NOINDEX, NOFOLLOW"))
                        continue;

                    // get name
                    names.Add(await GetName(document.DocumentNode.Descendants(), shop.Tags.NameTag, category.Category.Name));
                    // get price
                    // get characteristics

                }
                var pages = await brovser.PagesAsync();
                foreach (var item in pages)
                {
                    await item.CloseAsync();
                }
            }

            return new { pageLinks, names };
        }

        public async Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodUrlTag) && !x.HasClass("_next")).ToList();

            return bodyTags;
        }

        public async Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag)
        {
            // select from nodes
            // all nodes with class {goodUrlTag}
            // take from every node attribute href and it value
            var links = bodyTags.Select(x => x.Descendants().Where(d => d.HasClass(goodUrlTag)).FirstOrDefault(f => f.Attributes.Any(a => a.Name == "href")).GetAttributeValue("href", string.Empty)).ToList();

            return links;
        }

        public async Task<string> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName)
        {
            // select from nodes
            // all nodel with class {nameTag}
            // take from every node InetText and replace {categoryName} and trim
            var name = bodyTags.FirstOrDefault(x => x.HasClass(nameTag)).InnerHtml.Replace(categoryName, "").Trim();

            return name;
        }

        public async Task<string> GetHtml(string link)
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

        public async Task<object> GetPrice()
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetSpecification()
        {
            throw new NotImplementedException();
        }
    }
}

