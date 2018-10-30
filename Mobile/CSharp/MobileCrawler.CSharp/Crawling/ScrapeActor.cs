using System.Linq;
using Akka.Actor;
using HtmlAgilityPack;

namespace MobileCrawler.CSharp.Crawling
{
    public class ScrapeActor : ReceiveActor
    {
        public ScrapeActor(IActorRef parent)
        {
            Receive<Scrape>(msg => OnReceiveScrape(msg));
            Receive<ScrapeResult>(msg => parent.Forward(msg));
        }

        private void OnReceiveScrape(Scrape msg)
        {
            var document = new HtmlWeb().Load(msg.Url);

            var title = document.DocumentNode.SelectSingleNode("//head/title").InnerText;
            var links = document.DocumentNode.SelectNodes("//a")
                                .Select(link => link.GetAttributeValue("href", ""))
                                .Where(href => !string.IsNullOrWhiteSpace(href))
                                .ToList();

            Self.Tell(new ScrapeResult(msg.Url, title, links));
        }
    }
}