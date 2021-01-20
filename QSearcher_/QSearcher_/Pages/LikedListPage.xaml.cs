
using QSearcher_.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LikedListPage : ContentPage
    {

        List<Event> likedEvents = new List<Event>();
        public static bool flag = false;
        public LikedListPage()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                if (!flag)
                    mainGrid.Children.Remove(backArrowGrid);
                else
                    flag = false;
                return;
            }
            var a = CosmosLoad.GetObjects<Event>().Result;
            a.RemoveAll(e => e.Users.Count == 0 || e.Users.TrueForAll(u => CosmosLoad.GetObject<PersonLogin>(u).Result.Email == null));
            LikedList.ItemsSource = a;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            if (flag)
                mainGrid.Children.Add(backArrowGrid);
            else
                mainGrid.Children.Remove(backArrowGrid);
        }
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                await DisplayAlert("Вы не можете просмотреть информацию", null, null, "OK");

                return;
            }
            if (e.Item == null)
                return;
            var temp = ((ListView)sender).SelectedItem as Event;
            //List<string> userEmail = new List<string>();
            //List<string> userName= new List<string>();
            Dictionary<string, string> user = new Dictionary<string, string>();

            List<PersonLogin> users = new List<PersonLogin>();
            var details = ((ListView)sender).SelectedItem as Event;
            foreach (var item in details.Users)
            {
                users.Add(CosmosLoad.GetObject<PersonLogin>(item).Result);
            }
            if (users.Count == 0)
                await DisplayAlert("Проблемы с подключением", null, "ОК");
            else
            {
                foreach (var it in users)
                {
                    if (!it.LikeCheck)
                    {
                        if (!user.ContainsKey(it.Email))
                            if (it.Email != null && it.Email != "" && it.Email != "empty")
                                user.Add(it.Email, it.Name);
                    }
                }
            }
            if (user.Count == 0)
            {
                Event ver = CosmosLoad.GetObject<Event>(temp.Title).Result;
                CosmosLoad.DeleteObject<Event>(ver);

                OnAppearing();
            }
            else
            {
                List<string> lst = new List<string>();
                foreach (var item in user.Keys)
                {
                    lst.Add(user[item]);
                }
                var result = await DisplayActionSheet("Лайкнувшие", null, null, lst.ToArray());
                if (PersonLogin.EmailStatic != "empty" && PersonLogin.EmailStatic != null && result != null)
                {
                    foreach (var it in user.Keys)
                    {
                        if (user[it].Equals(result))
                        {
                            SendMessage(it, temp.Title);
                            return;
                        }

                    }
                }
                else
                    await DisplayAlert("Вы не можете отправить сообщение!", "Вы должны войти в систему", "ОК");
            }
        }
        async void SendMessage(string email, string title)
        {
            if (email == null)
                return;
            var result = await DisplayAlert("Подтвердить действие", "Вы точно хотите отправить сообщение данному пользователю?", "Нет", "Да");
            if (!result)
            {
                try
                {

                    MailAddress from = new MailAddress("no-reply@qsearcher.com", PersonLogin.EmailStatic);
                    MailAddress to = new MailAddress(email);
                    MailMessage m = new MailMessage(from, to);
                    m.Body = PersonLogin.NameStatic + " приглашает Вас посетить мероприятие : " + title + ".";
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("qsearcher.app@gmail.com", "10915610");
                    smtp.EnableSsl = true;
                    smtp.Send(m);

                }
                catch (Exception)
                {
                    await DisplayAlert("Отсутствует подключение к сети {" + email + "}", null, null, "OK");
                }
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            flag = false;
        }
    }
}
