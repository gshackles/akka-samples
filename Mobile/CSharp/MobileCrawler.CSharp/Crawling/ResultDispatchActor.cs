using Akka.Actor;

namespace MobileCrawler.CSharp.Crawling
{
    public class ResultDispatchActor : ReceiveActor
    {
        public ResultDispatchActor(MainViewModel viewModel) =>
            Receive<ScrapeResult>(result => viewModel.Results.Add(result));
    }
}