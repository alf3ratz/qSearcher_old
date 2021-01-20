
using QSearcher_.Data;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyListPage : ContentPage
    {
        public static List<Event> events;
        public static List<string> selected = new List<string>();
        public static MyListPage ev;
        public static DateTime pick = DateTime.Now;
        public MyListPage()
        {

            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            events = ContentManager.GetActualEvents();
            ev = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //events = ContentManager.GetActualEvents();
            eventsList.ItemsSource = events;
            ContentManager.ActualEvents = events;
            if (selected.Count != 0)
            {
                string select = string.Join(",", selected);
                eventsList.ItemsSource = ContentManager.GetFiltredEvents(select);
            }
            selected = new List<string>();
        }
        public async void Navigator(Event e)
        {
            await Navigation.PushModalAsync(new Detail(e));
        }
        async private void eventsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var details = ((ListView)sender).SelectedItem as Event;
            await Navigation.PushModalAsync(new Detail(details));
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            var result = await DisplayActionSheet("Фильтрация", null, null, "Разлвечения", "Кино", "Выставки", "Бизнесс",
                "Концерты", "Обучение", "Мода и стиль", "Фестивали", "Другие категории");

            switch ((string)result)
            {
                case "Развлечения":

                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("entertainment");

                    break;
                case "Выставки":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("exhibition");

                    break;
                case "Бизнесс":

                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("business-events");
                    break;
                case "Кино":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("cinema");
                    break;
                case "Концерты":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("concert");
                    break;
                case "Обучение":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("education");
                    break;
                case "Мода и стиль":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("fashion");
                    break;
                case "Фестивали":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("festival");
                    break;
                case "Дети":
                    eventsList.ItemsSource = ContentManager.GetFiltredEvents("kids");
                    break;
                case "Другие категории":
                    await Navigation.PushModalAsync(new SettingsPage());
                    break;
            }
        }
        private async void dateSort_Completed(object sender, EventArgs e)
        {
            DateTime n;
            if (!DateTime.TryParse(dateSort.Text + " 0:00:00", out n))
            {
                await DisplayAlert("Неправильный формат даты!", null, null, "OK");
                dateSort.Text = "";
            }
            else
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                    return;
                }
                pick = n;
                events = ContentManager.GetActualEvents(n);

                OnAppearing();
                dateSort.Text = "";
            }
        }
    }
}
