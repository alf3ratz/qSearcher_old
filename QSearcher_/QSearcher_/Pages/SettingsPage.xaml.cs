using QSearcher_.Data;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            MyListView.ItemsSource = ContentManager.Categories;
        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
            MyListPage.selected = (from category in ContentManager.Categories 
                                   where category.Selected select category.Slug).ToList();
            ContentManager.Categories.All(c => c.Selected = false);
            await Navigation.PopModalAsync();

            }
        }

        private void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem == null)
                return;
            Category temp = (Category)((ListView)sender).SelectedItem;
            temp.Selected = !temp.Selected;
        }
    }
}
