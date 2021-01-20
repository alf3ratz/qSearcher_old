using Newtonsoft.Json;
using System.Collections.Generic;

namespace QSearcher_.Data
{
    public class Event
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("users")]
        public List<string> Users = new List<string>();

        public string ShortTitle { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Address { get; set; }
        public string BodyText { get; set; }
        public bool HasCoords { get; set; }
        public static bool Check = true;


        public Event(string title, string desc, string pic, double lat, double lon, bool check, string dataStart, string dataEnd, string address, string bodyText)
        {

            Picture = pic;
            Title = title;
            DateStart = dataStart;
            DateEnd = dataEnd;
            Description = desc;
            Lat = lat;
            Lon = lon;
            HasCoords = check;
            Address = address;
            BodyText = bodyText;
        }
        public Event(string title, string desc, string pic, bool check, string dataStart, string dataEnd, string bodyText)
        {
            Picture = pic;
            Title = title;
            Description = desc;
            HasCoords = check;
            DateStart = dataStart;
            DateEnd = dataEnd;
            BodyText = bodyText;

        }
        public Event() { }

        public Event(string id, string title, List<string> users)
        {
            Id = id;
            Title = title;
            Users = users;
        }
    }
}
