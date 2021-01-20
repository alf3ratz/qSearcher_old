
using QSearcher_.Data;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonPage : ContentPage
    {
        string[] cities = new string[5] { "Москва", "Санкт-Петербург", "Нижний Новгород", "Казань", "Екатиринбург" };
        public static bool likeCheck = true;
        
        List<string> pickedCities = new List<string>();
        public PersonPage()
        {
            InitializeComponent();
            if (Preferences.ContainsKey("cities"))
            {
                string[] str = Preferences.Get("cities", "default").Split(',');
                //foreach (var item in str)
                   // cityShow.Text += item + Environment.NewLine;
            }
            else
                //cityShow.Text = "Москва";
            if (Preferences.ContainsKey("like"))
                likeCheck = Preferences.Get("like", false);
        }
        private void signOut_button_Clicked(object sender, EventArgs e)
        {
            PersonLogin.EmailStatic = "empty";
            PersonLogin.NameStatic = "empty";
            PersonLogin.PhotoUrlStatic = "empty";
            Preferences.Set("userName", "empty");
            this.OnAppearing();
        }
        protected override void OnAppearing()
        {
            if (likeCheck)
                liker.IsToggled = true;
            else
                liker.IsToggled = false;
            if (PersonLogin.NameStatic != "empty")
                userName.Text = Preferences.Get("userName", "default");
            else
                userName.Text = "-----";
        }
        private async void cityPick_Clicked(object sender, EventArgs e)
        {
            var picked = await DisplayActionSheet("Выберите город", null, null, cities);
            switch (picked)
            {
                case "Москва":
                    if (!ContentManager.Locations.Contains("msk"))
                    {
                        ContentManager.Locations.Add("msk");
                        OnAppearing();
                    }
                    break;
                case "Санкт-Петербург":
                    if (!ContentManager.Locations.Contains("spb"))
                    {
                        ContentManager.Locations.Add("spb");
                        OnAppearing();
                    }
                    break;
                case "Нижний Новгород":
                    if (!ContentManager.Locations.Contains("nnv"))
                    {
                        ContentManager.Locations.Add("nnv");
                        OnAppearing();
                    }
                    break;
                case "Казань":
                    if (!ContentManager.Locations.Contains("kzn"))
                    {
                        ContentManager.Locations.Add("kzn");
                        OnAppearing();
                    }
                    break;
                case "Екатиринбург":
                    if (!ContentManager.Locations.Contains("ekb"))
                    {
                        ContentManager.Locations.Add("ekb");
                        OnAppearing();
                    }
                    break;
            }
            Preferences.Set("city", cityPick.Text);
        }
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var details = ((Switch)sender);
            if (e.Value)
            {
                details.OnColor = Color.Green;
                details.ThumbColor = Color.Green;
            }
            else
            {
                details.OnColor = System.Drawing.Color.Red;
                details.ThumbColor = Color.Red;
            }
        }
       
        private void liker_Toggled(object sender, ToggledEventArgs e)
        {
            var details = ((Switch)sender);
            if (e.Value)
            {
                details.OnColor = Color.Green;
                details.ThumbColor = Color.Green;
                likeCheck = true;
                Preferences.Set("like", likeCheck);
            }
            else
            {
                details.OnColor = Color.Red;
                details.ThumbColor = Color.Red;
                likeCheck = false;
                Preferences.Set("like", likeCheck);

            }
        }

        private async void FavPage_Clicked(object sender, EventArgs e)
        {
            LikedListPage.flag = true;
            await Navigation.PushModalAsync(new LikedListPage());
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            string temp = string.Join(",", ContentManager.Locations);
            Preferences.Set("location", temp);
            MyListPage.events = ContentManager.GetActualEvents(MyListPage.pick);
            for (int i = 0; i < pickedCities.Count; i++)
            {
                //cityShow.Text += pickedCities[i] + Environment.NewLine;
            }
            string val = string.Join(",", pickedCities);
            Preferences.Set("cities", val);

             pickedCities = new List<string>();
            OnAppearing();
        }
    }
}