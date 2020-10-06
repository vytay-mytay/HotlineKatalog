using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.InternalModels;
using HotlineKatalog.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Services
{
    public class EldoradoParseService : AbstractParse
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddDBService _addDBService;

        public EldoradoParseService(IUnitOfWork unitOfWork, IAddDBService addDBService) : base()
        {
            _unitOfWork = unitOfWork;
            _addDBService = addDBService;
        }

        public override async Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag)
        {
            var links = bodyTags.Select(d => d.Descendants()
                                            .FirstOrDefault(c => c.HasClass(goodUrlTag))
                                            .ChildNodes
                                            .FirstOrDefault(c => c.Name == "a")
                                            .GetAttributeValue("href", string.Empty)).ToList();

            return links;
        }

        public override async Task<string> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName)
        {
            // select from nodes
            // all nodel with class {nameTag}
            // take from every node InetText and replace {categoryName} and trim
            var name = bodyTags.FirstOrDefault(x => x.HasClass(nameTag)).InnerText.Replace(categoryName, "").Replace("вбудовуваний", "").Trim().ToLower();

            return name;
        }

        public override async Task<string> GetNextPageLink(string html, string nextPageTag)
        {
            // click on div with "to-left"

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var nextPage = document.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass(nextPageTag));

            var link = nextPage.Descendants().FirstOrDefault(f => f.Attributes.Any(a => a.Name == "href")).GetAttributeValue("href", string.Empty);

            return link;
        }

        public override async Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodUrlTag)).ToList();

            return bodyTags;
        }

        public override async Task<int> GetPrice(IEnumerable<HtmlNode> bodyTags, string priceTag)
        {
            // get attribute "content" with price
            var price = int.Parse(bodyTags.FirstOrDefault(d => d.HasClass(priceTag) && d.Attributes.Any(a => a.Name == "content"))
                                            .GetAttributeValue("content", string.Empty));

            return price;
        }

        public override async Task<Dictionary<string, string>> GetSpecification(IEnumerable<HtmlNode> bodyTags, string specificationTag)
        {
            // li with 2 spans and one of it with class "attribute-val"

            // specificationTag == general-characteristic

            //var list = bodyTags.Where(d => d.HasClass(specificationTag) && d.Name == "li" && d.ChildNodes.All(c => c.Name == "span"));

            var test = bodyTags.Select(b => b.ChildNodes.FirstOrDefault(f => f.HasClass(specificationTag)));

            //var test1 = test.Where(t => t.Name == "li").SelectMany(t => t.ChildNodes.Where(c => c.Name == "span"));

            var test1 = test.Select(t => t.ChildNodes.FirstOrDefault(c =>c!= null && c.Name == "li" && c.ChildNodes.All(a => a.Name == "span")));

            var response = new Dictionary<string, string>();

            foreach (var item in test1)
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

            //foreach (var item in list)
            //{
            //    var key = item.Descendants().FirstOrDefault(x => x.HasClass("title")).InnerText.Trim();
            //    var value = item.Descendants().FirstOrDefault(x => x.HasClass("value")).InnerText.Trim();

            //    var canNotAdd = false;

            //    if (response.Any(x => x.Key == key))
            //        canNotAdd = true;

            //    while (canNotAdd)
            //    {
            //        key += '1';
            //        if (!response.Any(x => x.Key == key))
            //            canNotAdd = false;
            //    }

            //    response.Add(key, value);
            //}

            return response;
        }

        public override async Task Parse()
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

                int i = 1;

                while (doing)
                {
                    //string page = await GetHtml(brovser, pageUrl);
                    string page = await GetHtml(@"C:\Users\Шашлык с пивасом\source\repos\vytay-mytay\hakayaonline-backend\123.html");

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
                            //var itemHtml = await GetHtml(brovser, itemLink);
                            var itemHtml = await GetHtml(@"C:\Users\Шашлык с пивасом\source\repos\vytay-mytay\hakayaonline-backend\1234.html");

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

                        pageUrl = await GetNextPageLink(page, shop.Tags.NextPageTag);
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
