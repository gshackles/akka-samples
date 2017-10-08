using System.Collections.Generic;

namespace MobileCrawler.CSharp.Crawling
{
    public class Scrape 
    {
        public string Url { get; }

        public Scrape(string url) => Url = url;
    }

    public class DownloadUrlResult
    {
        public string Html { get; }

        public DownloadUrlResult(string html) => Html = html;
    }

    public class ScrapeResult
    {
        public string Url { get; }
        public string Title { get; }
        public IList<string> LinkedUrls { get; }

        public ScrapeResult(string url, string title, IList<string> linkedUrls)
        {
            Url = url;
            Title = title;
            LinkedUrls = linkedUrls;
        }
    }
}
