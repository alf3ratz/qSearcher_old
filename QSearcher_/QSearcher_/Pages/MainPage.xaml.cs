using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using QSearcher_.Data;

namespace QSearcher_.Pages
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        Event eve = null;
        Event first;
        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            first = ContentManager.GetEventOfTheDay();
            if (Event.Check)
            {
                ContentManager.GetActualEvents();
                Event.Check = false;
            }
            contactsListView.ItemsSource = ReturnFirst();
            LovedPage.LoaderLoved();//разблочить
            eventOfTheDayPicture.Source = first.Picture;
            eventOfTheDayLabelDesc.Text = first.Description;
            eventOfTheDayLabel.Text = first.Title;
            if (PersonLogin.EmailStatic == null)
            {
                PersonLogin.NameStatic = Preferences.Get("userName", "error");
                PersonLogin.EmailStatic = Preferences.Get("userEmail", "error");
                PersonLogin.PhotoUrlStatic = Preferences.Get("userPhoto", "error");
            }
            CosmosLoad.InsertObject(new PersonLogin(PersonLogin.NameStatic, PersonLogin.EmailStatic,
                PersonLogin.PhotoUrlStatic));
            ReturnEarlier();
            MyListPage.events = ContentManager.ActualEvents;
        }
        static List<Event> ReturnFirst()
        {
            List<Event> lst = new List<Event>();
            for (int i = 0; i < 3; i++)
            {

                lst.Add(ContentManager.ActualEvents[i]);
            }
            return lst;
        }
        private void ReturnEarlier()
        {
            try
            {

                if (LovedPage.FavoritesList.Count != 0)
                {
                    System.DateTime d = LovedPage.FavoritesList.Min(e => System.DateTime.Parse(e.DateStart));
                    eve = LovedPage.FavoritesList.Find(e => System.DateTime.Parse(e.DateStart) == d);
                    if (eve != null)
                    {
                        earlyEvent.Opacity = 1;
                        one.Opacity = 1;
                        three.Opacity = 1;
                        earlyEvent.Source = eve.Picture;
                        earlyEventTitle.Text = eve.ShortTitle;
                    }

                }
                else
                {
                    earlyEventTitle.Text = "СПИСОК БЛИЖАЙШИХ СОБЫТИЙ ПУСТ";
                    earlyEvent.Opacity = 0;
                    one.Opacity = 0.2;
                    two.Opacity = 0.2;
                    three.Opacity = 0.2;
                }
            }
            catch (Exception)
            {
                earlyEventTitle.Text = "СПИСОК БЛИЖАЙШИХ СОБЫТИЙ ПУСТ";
                one.Opacity = 0.2;
                two.Opacity = 0.2;
                three.Opacity = 0.2;
                earlyEvent.Opacity = 0;

            }
        }
        protected override async void OnAppearing()
        {
            try
            {

                if (PersonLogin.splashCheck)
                {
                    base.OnAppearing();
                    await Task.Delay(2000);

                    await Task.WhenAll(SplashGrid.FadeTo(0, 300),
                        Logo.ScaleTo(10, 2000));
                    PersonLogin.splashCheck = false;
                    MainGrid.Children.Remove(SplashGrid);
                }
                else
                {
                    ReturnEarlier();

                    base.OnAppearing();
                    MainGrid.Children.Remove(SplashGrid);
                }
            }
            catch (Exception)
            {
                one.Opacity = 1;
                two.Opacity = 1;
                three.Opacity = 1;

            }
        }

        private async void contactsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            else
            {
                var details = ((ListView)sender).SelectedItem as Event;
                await Navigation.PushModalAsync(new Detail(details));
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            else
            {
                if (eve != null)
                    await Navigation.PushModalAsync(new Detail(eve));
                else
                    await DisplayAlert("У вас нет ближайших событий", null, null, "OK");
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            else
            {
                if (first != null)
                    await Navigation.PushModalAsync(new Detail(first));
                else
                    await DisplayAlert("Сегодня нет популярного события", null, null, "OK");
            }
        }
    }
}
