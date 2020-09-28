using Flurl.Http;
using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Services.Interface;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Service
{
    public class EldoradoParseService : IParseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EldoradoParseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> Parse()
        {
            WebResponse response = null;
            StreamReader reader = null;

            var shop = _unitOfWork.Repository<Shop>().Get(x => x.Name == "Åכüהמנאהמ")
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .Include(x => x.Tags)
                                                        .FirstOrDefault();

            object test1 = null;

            foreach (var category in shop.Categories)
            {
                string page = "";

                try
                {
                    //var web = new HtmlWeb();
                    //var test = web.Load(shop.Url + category.Url).Encoding;


                    //using (WebClient client = new WebClient())
                    //{
                    //    page = client.DownloadString(new Uri(shop.Url + category.Url));
                    //    //page = Encoding.GetEncoding("ISO-8859-1").GetString(client.DownloadData(new Uri(shop.Url + category.Url)));
                    //}

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://comfy.ua/ua/refrigerator/");
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=ISO-8859-1";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
                    //request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
                    response = await request.GetResponseAsync();
                    reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
                    page = await reader.ReadToEndAsync();

                    var page1 = await "https://eldorado.ua/holodilniki/c1061560/".GetStringAsync();

                    //using var client = new HttpClient();
                    //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");

                    //page = await client.GetStringAsync("https://eldorado.ua/holodilniki/c1061560/");
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

                test1 = await GetPageItems(page, shop.Tags.GoodUrlTag);
                var test2 = await GetName(page, shop.Tags.NameTag, category.Category.Name);
            }

            return test1;
        }

        public async Task<object> GetPageItems(string html, string goodUrlTag)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var links = new List<string>();
            //var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodUrlTag));
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass("product-item__name"));

            StringBuilder body = new StringBuilder();
            foreach (var link in bodyTags/*.Select(x => x.SelectNodes("//a[@href]"))*/)
            {
                //links.Add(link.SelectNodes("//a[@href]").FirstOrDefault().GetAttributeValue("href", string.Empty));
                links.Add(link.ChildNodes.AsEnumerable().FirstOrDefault(x => x.Attributes.Any(x => x.Name == "href")).GetAttributeValue("href", string.Empty));
            }

            return links;
        }

        public async Task<object> GetName(string html, string nameTag, string categoryName)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var title = new List<string>();
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(nameTag));

            StringBuilder body = new StringBuilder();
            foreach (var item in bodyTags)
            {
                //item.InnerText
                //if (item.Descendants().Any(x => x.HasClass(titleTag)))
                //{
                //    item.Descendants().FirstOrDefault(x => x.HasClass(titleTag)).SelectNodes("//div").ToList().ForEach(x =>
                //    {
                //        title += x.InnerText + "- ";
                //    });
                //}

                //item.Descendants().FirstOrDefault(x => x.HasClass(titleTag)).SelectNodes("//div").ToList().ForEach(x =>
                //{
                //    title += x.InnerText + "- ";
                //});

                //if (item.Descendants().Any(x => x.HasClass(priceTag)))
                //{
                //    item.Descendants().FirstOrDefault(x => x.HasClass(priceTag)).SelectNodes("//span").ToList().ForEach(x =>
                //    {
                //        body.AppendLine(' ' + x.InnerText);
                //    });
                //}

                ////body.AppendLine(item.InnerText);
                //item.SelectNodes("//span").ToList().ForEach(x =>
                //{
                //    
                //});
            }

            return new
            {
                Names = title
            };
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

