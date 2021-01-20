
using Newtonsoft.Json;
using QSearcher_.Data;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LovedPage : ContentPage
    {
        public static List<Event> FavoritesList = new List<Event>();
        public static LovedPage lov;
        public LovedPage()
        {
            InitializeComponent();
            lovedList.ItemsSource = FavoritesList;
            lov = this;
        }
        public void Meth() { OnAppearing(); }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            lovedList.ItemsSource = FavoritesList;
            if (FavoritesList.Count != 0)
            {
                favouritePage.Children.Remove(nothingImage);
                lovedList.Opacity = 100;
            }
            else
            {
                favouritePage.Children.Add(nothingImage);
                lovedList.Opacity = 0;
            }
        }

        public static void LoaderLoved()
        {
            var list = Xamarin.Essentials.Preferences.Get("favorites", null);
            FavoritesList = list != null ? JsonConvert.DeserializeObject<List<Event>>(list) : new List<Event>();
        }
        public static void SaveFavorites()
        {
            var list = JsonConvert.SerializeObject(FavoritesList);
            Xamarin.Essentials.Preferences.Set("favorites", list);
        }
        private async void lovedList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var details = ((ListView)sender).SelectedItem as Event;
            await Navigation.PushModalAsync(new Detail(details));
            ((ListView)sender).SelectedItem = null;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
