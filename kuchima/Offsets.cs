using System.Net;
using Newtonsoft.Json;

namespace kuchima
{
    public class Offsets
    {
        public static dynamic hazedumper;
        private const string url = "https://raw.githubusercontent.com/frk1/hazedumper/master/csgo.json";

        public static void Load()
        {
            WebClient wc = new();
            hazedumper = JsonConvert.DeserializeObject(wc.DownloadString(url));
            wc.Dispose();
        }
    }
}
