using Newtonsoft.Json;
using System.Text;

namespace DiscordBotOnDScharp.Config
{
    internal class JSONReader
    {
        public string Token { get; set; }
        public string Prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("Config.json", new UTF8Encoding(false)))
            {
                string json = await sr.ReadToEndAsync();
                ConfigJSON obj = JsonConvert.DeserializeObject<ConfigJSON>(json);

                Token = obj.Token; 
                Prefix = obj.Prefix;
            }
        }
    }
    internal sealed class ConfigJSON
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
