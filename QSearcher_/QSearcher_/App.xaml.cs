
using Xamarin.Forms;

namespace QSearcher_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new Pages.AppShell();
        }

        protected override void OnStart()
        {
        }

        
    }
}
