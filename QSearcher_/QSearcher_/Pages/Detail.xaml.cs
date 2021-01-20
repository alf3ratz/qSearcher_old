
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QSearcher_.Data;
using Xamarin.Essentials;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty("Name", "name")]
    public partial class Detail : ContentPage
    {
        string Title
        {
            set
            {
                BindingContext = ContentManager.ActualEvents.FirstOrDefault(m => m.Title == Uri.UnescapeDataString(value));
            }
        }
        string Address { get; set; }
        Data.Event CurrentEvent { get; set; }
        public Detail()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
        }
        public Detail(Data.Event e)
        {
            InitializeComponent();
            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = e.BodyText.Replace("<p>", "<p style=\"font-size:90%;\">");
            bodyView.Source = htmlSource;
            var htmlSource1 = new HtmlWebViewSource();
            htmlSource1.Html = e.Description.Replace("<p>", "<p style=\"font-size:90%;\">");
            descripView.Source = htmlSource1;
            shower.Source = (e.Picture);
            Title = e.Title;
            titleLabel.Text = e.Title;
            CurrentEvent = e;
           
            if (e.DateEnd.Equals("31.12.9998 21:00"))
                end.Text = "";
            else
                end.Text = "Конец: " + e.DateEnd;
            this.address.Text = e.Address;
            if (Address == null && DateTime.Parse(e.DateStart) > DateTime.Parse("01.03.2020"))
                address.Text = "Из-за коронавируса событие проводится онлайн.";
            
            else if (Address == null && DateTime.Parse(e.DateStart) < DateTime.Parse("01.03.2020"))
                address.Text = "";
            else if(e.DateStart.Equals("02.01.0001 21:30"))
                address.Text = "Из-за коронавируса событие проводится онлайн.";
            else
                address.Text = "Адресс: " + e.Address;
            if (e.DateStart.Equals("02.01.0001 21:30"))
            {
                start.Text = "Начало: " + "сегодня в " + e.DateStart.Replace("02.01.0001", "");
                address.Text = "Из-за коронавируса событие проводится онлайн.";

            }
            else
                start.Text = "Начало: " + e.DateStart;
        }
        private async void PlusButton_Clicked(object sender, EventArgs e)
        {
            if (!LovedPage.FavoritesList.Contains(CurrentEvent))
            {
                LovedPage.FavoritesList.Add(CurrentEvent);
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {

                    if (PersonLogin.NameStatic != "empty" && PersonLogin.NameStatic != "error")
                    {
                        try
                        {
                        var ev = CosmosLoad.GetObject<Event>(CurrentEvent.Title).Result;
                        if (ev != null)
                            CurrentEvent.Users = ev.Users;
                        var user = CosmosLoad.GetObject<PersonLogin>(PersonLogin.EmailStatic).Result;
                        if (user != null && !CurrentEvent.Users.Contains(user.Id))

                            CurrentEvent.Users.Add(user.Id);
                        await CosmosLoad.UpdateEvent(CurrentEvent);

                        }
                        catch (Exception)
                        {
                            await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                    await DisplayActionSheet("Cобытие было добавлено в список избранного",
                        "Другие пользователи не увидят вашу метку",
                         null, null, null);
                }


                LovedPage.SaveFavorites();
            }
        }

        async private void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        private async void MinusButton_Clicked(object sender, EventArgs e)
        {
            Event evv = LovedPage.FavoritesList.Find(eve => eve.Title == CurrentEvent.Title);
            if (evv != null)
            {
                LovedPage.FavoritesList.Remove(evv);
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    if (PersonLogin.NameStatic != "empty" && PersonLogin.NameStatic != "error")
                    {
                        try
                        {

                        var ev = CosmosLoad.GetObject<Event>(CurrentEvent.Title).Result;
                        if (ev != null)
                            CurrentEvent.Users = ev.Users;
                        var user = CosmosLoad.GetObject<PersonLogin>(PersonLogin.EmailStatic).Result;
                        if (CurrentEvent.Users.Contains(user.Id))
                            CurrentEvent.Users.Remove(user.Id);
                        await CosmosLoad.UpdateEvent(CurrentEvent);
                        }
                        catch (Exception)
                        {
                            await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                    await DisplayActionSheet("Вы удалили событие из списка избранных",
                       "Данные не отобразятся у других пользователей", null, null, null);
                }
                LovedPage.SaveFavorites();
            }
        }
    }
}