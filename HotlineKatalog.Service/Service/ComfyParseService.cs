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
    public class ComfyParseService : IParseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ComfyParseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> Parse()
        {
            var shop = _unitOfWork.Repository<Shop>().Get(x => x.Name == "Comfy")
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .Include(x => x.Tags)
                                                        .FirstOrDefault();

            List<string> pageLinks = null;
            List<string> names = null;

            foreach (var category in shop.Categories)
            {
                string page = await GetHtml(category.Url);

                var bodyTags = await GetPageItems(page, shop.Tags.NameTag);
                pageLinks = await GetLinks(bodyTags, shop.Tags.GoodUrlTag);

                foreach (var itemLink in pageLinks)
                {
                    var itemHtml = await GetHtml(itemLink);

                    var document = new HtmlDocument();
                    document.LoadHtml(itemHtml);

                    // get name
                    await GetName(document.DocumentNode.Descendants());
                    // get characteristics
                }

                names = await GetName(bodyTags, shop.Tags.GoodUrlTag, category.Category.Name);
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

        public async Task<List<string>> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName)
        {
            // select from nodes
            // all nodel with class {nameTag}
            // take from every node InetText and replace {categoryName} and trim
            var names = bodyTags.Select(x => x.Descendants().Where(x => x.HasClass(nameTag)).FirstOrDefault().InnerText.Replace(categoryName, "").Trim()).ToList();

            return names;
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

