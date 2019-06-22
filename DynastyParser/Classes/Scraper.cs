using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Text;
using System.Threading.Tasks;

namespace DynastyParser.Classes
{
    class Scraper
    {
        private string baseUrl;


        public Scraper(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }


        /// <summary>
        /// Scrape links to chapters of a given manga
        /// </summary>
        /// <param name="mangaUrl"></param>
        /// <returns>List of chapter URLs</returns>
        public List<string> ScrapeChapters(string mangaUrl)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(mangaUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.ChildNodes;

            IEnumerable<HtmlNode> links = nodes.CssSelect("dl.chapter-list>dd>a");
            List<string> hrefs = links.Select(node => node.GetAttributeValue("href","")).ToList();
            hrefs = hrefs.Select(s => baseUrl + s).ToList();

            return hrefs;
        }

        /// <summary>
        /// Scrape chapter page to get URLs to all images
        /// </summary>
        /// <param name="chapterUrl"></param>
        /// <returns>List of image URLs</returns>
        public List<string> ScrapeImages(string chapterUrl)
        {
            List<string> images = new List<string>();

            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(chapterUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.ChildNodes;

            // Get all page numbers
            IEnumerable<HtmlNode> links = nodes.CssSelect("div.pages-list>a.page");
            List<string> ids = links.Select(node => node.InnerText).ToList();

            // Get current page image
            HtmlNode image = nodes.CssSelect("div#image>img").First();
            string imgSrc = image.GetAttributeValue("src", "");
            var splitSrc = imgSrc.Split('/');
            string baseImgSrc = string.Join("/", splitSrc.SkipLast(1));
            string extension = splitSrc.Last().Split('.').Last();
            
            foreach (var id in ids)
            {
                images.Add($"{baseUrl}{baseImgSrc}/{id}.{extension}");
            }

            return images;
        }
    }
}
