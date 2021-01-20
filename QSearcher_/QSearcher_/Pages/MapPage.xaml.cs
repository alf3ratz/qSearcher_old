
using QSearcher_.Data;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace QSearcher_.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        List<Event> positions = new List<Event>();
        public static double MyLat;
        public static double MyLon;
        public MapPage()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            for (int i = 0; i < MyListPage.events.Count; i++)
            {
                if (ContentManager.ActualEvents[i].HasCoords)
                {
                    Xamarin.Forms.Maps.Pin mapPin = new Xamarin.Forms.Maps.Pin
                    {
                        Type = Xamarin.Forms.Maps.PinType.Place,
                        Position = new Xamarin.Forms.Maps.Position(ContentManager.ActualEvents[i].Lat, ContentManager.ActualEvents[i].Lon),
                        Label = ContentManager.ActualEvents[i].Title,
                        Address = ContentManager.ActualEvents[i].Address
                    };
                    MyMap.Pins.Add(mapPin);
                    positions.Add(ContentManager.ActualEvents[i]);
                    mapPin.InfoWindowClicked += async (s, args) =>
                     {
                         foreach (var item in ContentManager.ActualEvents)
                         {
                             if (item.Title.Equals(((Xamarin.Forms.Maps.Pin)s).Label))
                             {
                                 try
                                 {
                                     await Navigation.PushModalAsync(new Detail(item));
                                 }
                                 catch (Java.Lang.RuntimeException)
                                 {
                                     await DisplayAlert("Ошибка выгрузки события", null, null, "OK");
                                     ContentManager.ActualEvents.Remove(item);
                                 }
                             }
                         }
                     };
                }
            }
            var myPos = new Xamarin.Forms.Maps.Pin
            {
                Type = Xamarin.Forms.Maps.PinType.SearchResult,
                Position = new Xamarin.Forms.Maps.Position(MyLat, MyLon),
                Label = "Me",
                Address = ""
            };
            MyMap.Pins.Add(myPos);
            Xamarin.Forms.GoogleMaps.Position posit = new Xamarin.Forms.GoogleMaps.Position(MyLat, MyLon);
            CameraPosition cameraPosition = new CameraPosition(posit, 18, 155, 65);
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            Xamarin.Forms.Maps.Position post = new Xamarin.Forms.Maps.Position(MyLat + 1, MyLon + 1);
            Xamarin.Forms.Maps.MapSpan sp = new Xamarin.Forms.Maps.MapSpan(post, MyLat, MyLon);
            MyMap.MoveToRegion(sp);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Отсутствует подключение к сети", null, null, "OK");
                return;
            }
            for (int i = 0; i < MyListPage.events.Count; i++)
            {
                if (ContentManager.ActualEvents[i].HasCoords)
                {
                    Xamarin.Forms.Maps.Pin mapPin = new Xamarin.Forms.Maps.Pin
                    {
                        Type = Xamarin.Forms.Maps.PinType.Place,
                        Position = new Xamarin.Forms.Maps.Position(ContentManager.ActualEvents[i].Lat, ContentManager.ActualEvents[i].Lon),
                        Label = ContentManager.ActualEvents[i].Title,
                        Address = ContentManager.ActualEvents[i].Address
                    };
                    MyMap.Pins.Add(mapPin);
                    positions.Add(ContentManager.ActualEvents[i]);
                    mapPin.InfoWindowClicked += async (s, args) =>
                    {
                        foreach (var item in ContentManager.ActualEvents)
                        {
                            if (item.Title.Equals(((Xamarin.Forms.Maps.Pin)s).Label))
                            {
                                try
                                {
                                    await Navigation.PushModalAsync(new Detail(item));
                                }
                                catch (Java.Lang.RuntimeException)
                                {
                                    await DisplayAlert("Ошибка выгрузки события", null, null, "OK");
                                    ContentManager.ActualEvents.Remove(item);
                                }
                            }
                        }
                    };
                }
            }
        }
    }
}