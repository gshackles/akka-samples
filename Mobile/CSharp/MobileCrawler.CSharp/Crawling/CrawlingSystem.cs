using System.Threading;
using Akka.Actor;

namespace MobileCrawler.CSharp.Crawling
{
    public static class CrawlingSystem
    {
        private static readonly ActorSystem _system;
        private static readonly IActorRef _coordinator;

        static CrawlingSystem()
        {
            _system = ActorSystem.Create("crawling-system");
            _coordinator = _system.ActorOf(Props.Create<CoordinatorActor>(), "coordinator");
        }

        public static void StartCrawling(string url, MainViewModel viewModel)
        {
            var props = Props.Create(() => new ResultDispatchActor(viewModel));
            var dispatcher = _system.ActorOf(props);

            _system.EventStream.Subscribe(dispatcher, typeof(ScrapeResult));

            _coordinator.Tell(new Scrape(url));
        }
    }
}