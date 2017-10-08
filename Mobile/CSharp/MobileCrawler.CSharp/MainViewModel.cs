using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MobileCrawler.CSharp.Crawling;
using Xamarin.Forms;

namespace MobileCrawler.CSharp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<ScrapeResult> Results { get; } = new ObservableCollection<ScrapeResult>();
        public ICommand StartCrawlingCommand { get; }

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Url)));
            }
        }

        public MainViewModel() =>
            StartCrawlingCommand = new Command(() => CrawlingSystem.StartCrawling(_url, this));
    }
}