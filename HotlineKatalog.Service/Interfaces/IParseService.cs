using HtmlAgilityPack;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Interfaces
{
    public interface IParseService
    {
        Task Parse();

        /// <summary>
        /// Get goods from page
        /// </summary>
        /// <param name="html"></param>
        /// <param name="goodUrlTag"></param>
        /// <returns></returns>
        Task<IEnumerable<HtmlNode>> GetPageItems(string html, string goodUrlTag);

        ///// <summary>
        ///// Get next page link
        ///// </summary>
        ///// <param name="html"></param>
        ///// <param name="nextPageTag"></param>
        ///// <returns></returns>
        //Task<string> GetNextPageLink(string html, string nextPageTag);

        /// <summary>
        /// Get links of goods
        /// </summary>
        /// <param name="bodyTags"></param>
        /// <param name="goodUrlTag"></param>
        /// <returns></returns>
        Task<List<string>> GetLinks(IEnumerable<HtmlNode> bodyTags, string goodUrlTag);

        /// <summary>
        /// Get name of good
        /// </summary>
        /// <param name="bodyTags"></param>
        /// <param name="nameTag"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        Task<string> GetName(IEnumerable<HtmlNode> bodyTags, string nameTag, string categoryName);

        /// <summary>
        /// Get html string from link
        /// </summary>
        /// <param name="brovser"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        Task<string> GetHtml(Browser brovser, string link);

        /// <summary>
        /// Start browser
        /// </summary>
        /// <returns></returns>
        Task<Browser> StartBrovser();

        /// <summary>
        /// Get price of good
        /// </summary>
        /// <param name="bodyTags"></param>
        /// <param name="priceTag"></param>
        /// <returns></returns>
        Task<int> GetPrice(IEnumerable<HtmlNode> bodyTags, string priceTag);

        /// <summary>
        /// Get good specification
        /// </summary>
        /// <param name="bodyTags"></param>
        /// <param name="specificationTag"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetSpecification(IEnumerable<HtmlNode> bodyTags, string specificationTag);
    }
}
