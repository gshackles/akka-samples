using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace MobileCrawler.CSharp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainViewModel();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}
