using HotlineKatalog.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotlineKatalog.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HotlineKatalogController : Controller
    {
        private readonly IParseService _parseService;

        public HotlineKatalogController(IParseService parseService)
        {
            _parseService = parseService;
        }

        // GET api/v1/chats/users
        /// <summary>
        /// Get users list for chat
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/chats/users?limit=20&amp;offset=0
        ///     
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>A chat list</returns>
        [HttpGet]
        public async Task<IActionResult> GetChatUsers(string link)
        {
            //var test2 = await _parseService.Parse();

            //WebResponse response = null;
            //StreamReader reader = null;

            //string test = "";

            //try
            //{
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            //    request.Method = "GET";
            //    request.ContentType = "text/html;charset=utf-8";
            //    //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            //    response = await request.GetResponseAsync();
            //    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //    test = await reader.ReadToEndAsync();
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    if (reader != null)
            //        reader.Close();
            //    if (response != null)
            //        response.Close();
            //}

            //var test2 = await GetContentFromTagAsync(test);

            return Ok();
        }

        private async Task<object> GetContentFromTagAsync(string html)
        {
            var goodTag = "goods-item-content";
            var titleTag = "title lp";
            var priceTag = "current-price h1";

            //return await Task.Run(() =>
            //{
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var title = "";
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodTag));

            int i = 0;

            StringBuilder body = new StringBuilder();
            foreach (var item in bodyTags)
            {
                //if (item.Descendants().Any(x => x.HasClass(titleTag)))
                //{
                //    item.Descendants().FirstOrDefault(x => x.HasClass(titleTag)).SelectNodes("//div").ToList().ForEach(x =>
                //    {
                //        title += x.InnerText + "- ";
                //    });
                //}

                item.Descendants().FirstOrDefault(x => x.HasClass(titleTag)).SelectNodes("//div").ToList().ForEach(x =>
                {
                    title += x.InnerText + "- ";
                });

                if (item.Descendants().Any(x => x.HasClass(priceTag)))
                {
                    item.Descendants().FirstOrDefault(x => x.HasClass(priceTag)).SelectNodes("//span").ToList().ForEach(x =>
                    {
                        body.AppendLine(' ' + x.InnerText);
                    });
                }

                ////body.AppendLine(item.InnerText);
                //item.SelectNodes("//span").ToList().ForEach(x =>
                //{
                //    
                //});
                i++;
            }

            return new { Title = title, Body = body.ToString(), Iterator = i };
            //});
        }



        #region Test

        private void IkeaParser(string html)
        {
            var goodTag = "range-revamp-product-compact";
            var titleTag = "range-revamp-header-section__title--small";
            var priceTag = "range-revamp-price__integer";

            //return await Task.Run(() =>
            //{
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var title = "";
            var bodyTags = document.DocumentNode.Descendants().Where(x => x.HasClass(goodTag));

            StringBuilder body = new StringBuilder();
            foreach (var item in bodyTags)
            {
                title += item.Descendants().FirstOrDefault(x => x.HasClass(titleTag)).InnerText + "- ";

                body.AppendLine(' ' + item.Descendants().FirstOrDefault(x => x.HasClass(priceTag)).InnerText);
            }

            //return new { Title = title, Body = body.ToString(), Url = link };
        }

        #endregion

    }
}
