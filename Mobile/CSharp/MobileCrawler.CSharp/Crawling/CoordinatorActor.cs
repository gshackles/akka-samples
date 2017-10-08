using Akka.Actor;
using Akka.Routing;

namespace MobileCrawler.CSharp.Crawling
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly IActorRef _crawlers;

        public CoordinatorActor()
        {
            _crawlers = Context.ActorOf(
                Props.Create(() => new ScrapeActor(Self)).WithRouter(new SmallestMailboxPool(10)));

            Receive<Scrape>(msg => _crawlers.Tell(msg));
            Receive<ScrapeResult>(msg => OnReceiveScrapeResult(msg));
        }

        private void OnReceiveScrapeResult(ScrapeResult result)
        {
            foreach (var url in result.LinkedUrls)
                _crawlers.Tell(new Scrape(url));

            if (!string.IsNullOrWhiteSpace(result.Title))
                Context.System.EventStream.Publish(result);
        }
    }
}
