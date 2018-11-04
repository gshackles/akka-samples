using Akka.Actor;
using Xamarin.Forms;

namespace MobileCrawler.CSharp.Crawling
{
    public class ResultDispatchActor : ReceiveActor
    {
        public ResultDispatchActor(MainViewModel viewModel) =>
            Receive<ScrapeResult>(result => Device.BeginInvokeOnMainThread(() => viewModel.Results.Add(result)));
    }
}