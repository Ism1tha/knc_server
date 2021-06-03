using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace KnC_Server
{
    class Country
    {
        public static string CountryByIP(string IP)
        {
            string _country = "NULL";
            //var url = "http://freegeoip.net/json/" + IP;
            //var url = "http://freegeoip.net/json/" + IP;
            string url = "http://api.ipstack.com/" + IP + "?access_key=156c45389d1e6c5fccbb78d909153233";
            var request = System.Net.WebRequest.Create(url);

            using (WebResponse wrs = request.GetResponse())
            using (Stream stream = wrs.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var obj = JObject.Parse(json);
                _country = (string)obj["country_code"];
                Console.WriteLine($"{_country}");
            }
            return _country;
        }
    }
}
