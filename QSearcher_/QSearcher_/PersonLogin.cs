
using Newtonsoft.Json;


namespace QSearcher_
{
   public class PersonLogin
    {
        [JsonProperty("Name")]
        public  string Name { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("PhotoUrl")]
        public  string PhotoUrl { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("LikeCheck")]
        
        public  bool LikeCheck { get; set; }
        public static  string NameStatic { get; set; }
        public static string EmailStatic { get; set; }
        public static string PhotoUrlStatic { get; set; }
        public static bool splashCheck = true;

        public PersonLogin(string name, string email, string url)
        {
            Name = name;
            Email = email;
            PhotoUrl = url;
            NameStatic = name;
            EmailStatic = email;
            PhotoUrlStatic = url;
        }
        public PersonLogin(string name, string email, string url,string id,bool like)
        {
            Name = name;
            Email = email;
            PhotoUrl = url;
            Id = id;
            NameStatic = name;
            EmailStatic = email;
            PhotoUrlStatic = url;
            LikeCheck = like;
            
        }
        public PersonLogin() { }
    }
}
