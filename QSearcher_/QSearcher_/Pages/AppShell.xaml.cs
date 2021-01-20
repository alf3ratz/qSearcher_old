
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}