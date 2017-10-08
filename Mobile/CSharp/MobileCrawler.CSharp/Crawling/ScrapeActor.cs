using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using AngleSharp;

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
            var config = Configuration.Default.WithDefaultLoader();

            BrowsingContext.New(config).OpenAsync(msg.Url).ContinueWith(request =>
            {
                var document = request.Result;
                var links = document.Links
                                    .Select(link => link.GetAttribute("href"))
                                    .ToList();

                return new ScrapeResult(document.Url, document.Title, links);
            }, TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
        }
    }
}