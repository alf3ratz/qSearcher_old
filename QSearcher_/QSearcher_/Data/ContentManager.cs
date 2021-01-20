using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.ObjectModel;

namespace QSearcher_.Data
{
    public static class ContentManager
    {
        public static List<string> Locations = new List<string>() { "msk" }; //!!!!!!!!!!!!!!!!! 
        public static List<Event> ActualEvents = new List<Event>();
        public static string location = "msk";//Xamarin.Essentials.Preferences.Get("location","empty");
        public static ObservableCollection<Category> Categories = new ObservableCollection<Category>
{
new Category("Бизнесс","business-events"),
new Category("Кино","cinema"),
new Category("Спектакли","theater"),
new Category("Развлечения","entertainment"),
new Category("Концерты","concert"),
new Category("Обучение","education"),
new Category("Выставки","exhibition"),
new Category("Экскурсии","tour"),
new Category("Мода и стиль","fashion"),
new Category("Фестивали","festival"),
new Category("Праздники","party"),
new Category("Детям","kids"),
new Category("Вечеринки","party"),
new Category("Фотография","photo"),
new Category("Квесты","quest"),
new Category("Отдых","recreation"),
new Category("Шопинг","shopping"),
new Category("Благотворительность","social-activity"),
new Category("Акции и скидки","stock"),
new Category("Ярмарки","yarmarki-razvlecheniya-yarmarki"),
new Category("Разное","other")
};

        /// <summary>
        /// Парсит событие
        /// </summary>
        /// <param name="o">Объект</param>
        /// <returns>Событие</returns>
        public static Event ParseEvent(JToken o)
        {
            var e = new Event()
            {
                Title = char.ToUpper(o["title"].ToString()[0]).ToString() + o["title"].ToString().Substring(1),
                Description = o["description"].ToString(),
                BodyText = o["body_text"].ToString(),
                Picture = o["images"][0]["image"].ToString(),
            };
            e.ShortTitle = o["short_title"].ToString() == "" ? e.Title :
            char.ToUpper(o["short_title"].ToString()[0]).ToString() + o["short_title"].ToString().Substring(1);
            int dateCount = o["dates"].Count();
            e.DateStart = (new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)o["dates"][dateCount - 1]["start"])).ToString("dd.MM.yyyy HH:mm");
            e.DateEnd = (new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)o["dates"][dateCount - 1]["end"])).ToString("dd.MM.yyyy HH:mm");
            try
            {
                var url = "https://kudago.com/public-api/v1.4/places/" + o["place"]["id"] + "/?fields=title,coords,address,short_title";
                var O = MakeRequest(url);

                double lat = double.Parse(O["coords"]["lat"].ToString());
                double lon = double.Parse(O["coords"]["lon"].ToString());
                string address = O["address"].ToString();
                e.Lat = lat;
                e.Lon = lon;
                e.Address = address;
                e.HasCoords = true;
            }
            catch
            {
                e.HasCoords = false;
            }
            return e;
        }
        /// <summary>
        /// Осуществляет API-запрос к KudaGO и возвращает объект
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <returns>Объект</returns>
        private static JObject MakeRequest(string url)
        {
            var req = WebRequest.Create(url);
            var resp = req.GetResponse();
            var resStream = resp.GetResponseStream();
            string temp = null;
            using (var sr = new StreamReader(resStream))
            {
                temp = sr.ReadToEnd();
            }
            JObject O = JObject.Parse(temp);
            return O;
        }
        public static Event GetEventOfTheDay()
        {
            var url = "https://kudago.com/public-api/v1.4/events-of-the-day/?" + location;
            var o = MakeRequest(url);

            var eurl = "https://kudago.com/public-api/v1.4/events/";
            eurl += o["results"][0]["object"]["id"];
            eurl += "/?fields=dates,title,short_title,place,categories,images,description,body_text";
            eurl += "&text_format=text&" + "location=" + location + "&expand=dates&order_by=-publication_date";
            var e = MakeRequest(eurl);
            return ParseEvent(e);
        }
        /// <summary>
        /// Возвращает список событий по запросу
        /// </summary>
        /// <param name="url">Запрос</param>
        /// <returns>Список событий</returns>
        public static List<Event> GetEvents(string url)
        {
            var events = new List<Event>();
            var o = MakeRequest(url);

            var count = o["results"].Count();
            for (int i = 0; i < count; i++)
            {
                events.Add(ParseEvent(o["results"][i]));
            }
            return events;
        }
        /// <summary>
        /// Возвращает список всех актуальных событий
        /// </summary>
        /// <returns>Список актуальных событий</returns>

        public static List<Event> GetActualEvents(DateTime date = new DateTime())
        {
            bool flag = date.ToString() == "01.01.0001 0:00:00";
            //if (date.ToString() == "01.01.0001 0:00:00" )
            //date = DateTime.Parse("01.01.2020");
            if (ActualEvents.Count == 0 || !flag)
            {
                ActualEvents = new List<Event>();
                var url = "https://kudago.com/public-api/v1.4/events/?";
                url += "fields=dates,title,short_title,place,categories,images,description,body_text";
                if (!flag)
                    url += "&actual_until=" + (date.Ticks / 10000000L - 62135596800L);
                else
                    url += "&actual_since=" + (DateTime.Now.Ticks / 10000000L - 62135596800L);
                url += "&text_format=html&page_size=99&expand=dates&order_by=-publication_date&location=";
                foreach (var l in Locations)
                {
                    var temp_url = url + l;
                    ActualEvents.AddRange(GetEvents(temp_url));
                }
            }
            return ActualEvents;
        }

        /// <summary>
        /// Включает в выдачу события из указанных категорий
        /// Можно несколько через запятую;
        /// Если категория указана со знаком минус, то события с такой категорией будут выключены из выдачи
        /// </summary>
        /// <param name="categories">строка с категориями</param>
        /// <returns>список</returns>
        public static List<Event> GetFiltredEvents(string categories, DateTime date = new DateTime())
        {
            bool flag = date.ToString() == "01.01.0001 0:00:00";
            var events = new List<Event>();
            var url = "https://kudago.com/public-api/v1.4/events/?";
            url += "fields=dates,title,short_title,place,categories,price,images,description,body_text";
            if (!flag)
                url += "&actual_until=" + (date.Ticks / 10000000L - 62135596800L);
            else
                url += "&actual_since=" + (DateTime.Now.Ticks / 10000000L - 62135596800L);
            url += "&text_format=html&page_size=99&expand=dates&order_by=-publication_date&";
            url += "&categories=" + categories + "&location=";
            foreach (var l in Locations)
            {
                var temp_url = url + l;
                events.AddRange(GetEvents(temp_url));
            }
            return events;
        }
        /// <summary>
        /// Возвращает рзультат поиска
        /// </summary>
        /// <param name="q">запрос</param>
        /// <returns>список</returns>

        public static List<Event> GetFoundEvents(string q)
        {
            var events = new List<Event>();
            var url = "https://kudago.com/public-api/v1.4/search/?";
            url += "q=" + q;
            url += "&page_size=99&" + "location=" + location + "&ctype=events";
            var o = MakeRequest(url);
            var count = o["results"].Count();
            for (int i = 0; i < count; i++)
            {
                var eurl = "https://kudago.com/public-api/v1.4/events/";
                eurl += o["resulrts"][i]["id"];
                eurl += "/?fields=dates,title,short_title,place,categories,images,description,body_text";
                eurl += "&text_format=html&" + "location=" + location + "&expand=dates&order_by=-publication_date";
                var e = MakeRequest(eurl);

                events.Add(ParseEvent(e));
            }
            return events;
        }

    }
}
