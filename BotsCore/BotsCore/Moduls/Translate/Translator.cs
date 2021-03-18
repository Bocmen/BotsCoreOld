using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Модуль перевода текста
    /// </summary>
    public class Translator : TranslateCore
    {
        private static Lang.LangTypes[] langs = new Lang.LangTypes[]
        {
                    Lang.LangTypes.ru,
                    Lang.LangTypes.en,
                    Lang.LangTypes.es,
                    Lang.LangTypes.he,
                    Lang.LangTypes.it,
                    Lang.LangTypes.nl,
                    Lang.LangTypes.ja,
                    Lang.LangTypes.ar,
                    Lang.LangTypes.zh,
                    Lang.LangTypes.de,
                    Lang.LangTypes.pl,
                    Lang.LangTypes.pt,
                    Lang.LangTypes.ro,
                    Lang.LangTypes.tr
        };
        public override uint MaxCharText => 800;
        public override Lang.LangTypes[] Langs => langs;

        private static string GetNameLang(Lang.LangTypes lang)
        {
            switch (lang)
            {
                case Lang.LangTypes.ru:
                    return "rus";
                case Lang.LangTypes.fr:
                    return "fra";
                case Lang.LangTypes.es:
                    return "spa";
                case Lang.LangTypes.he:
                    return "heb";
                case Lang.LangTypes.it:
                    return "ita";
                case Lang.LangTypes.nl:
                    return "dut";
                case Lang.LangTypes.ja:
                    return "jap";
                case Lang.LangTypes.en:
                    return "eng";
                case Lang.LangTypes.ar:
                    return "ara";
                case Lang.LangTypes.zh:
                    return "chi";
                case Lang.LangTypes.de:
                    return "ger";
                case Lang.LangTypes.pl:
                    return "pol";
                case Lang.LangTypes.pt:
                    return "por";
                case Lang.LangTypes.ro:
                    return "rum";
                case Lang.LangTypes.tr:
                    return "tur";
            }
            return "rus";
        }
        public override string Translate(string txt, Lang.LangTypes langTo, Lang.LangTypes? lang = null)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return txt;
            using (var client = new HttpClient())
            {
                try
                {
                    string s = $"{{\"input\":{JsonConvert.SerializeObject(txt)},\"from\":\"{GetNameLang((Lang.LangTypes)lang)}\",\"to\":\"{GetNameLang(langTo)}\",\"format\":\"text\",\"options\":{{\"origin\":\"reversodesktop\",\"sentenceSplitter\":false,\"contextResults\":false,\"languageDetection\":true}}}}";
                    string responseText = client.PostAsync("https://api.reverso.net/translate/v1/translation", new StringContent(s, Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync().Result;
                    return ((Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(responseText)).GetValue("translation").ToObject<Newtonsoft.Json.Linq.JArray>()[0].ToString();
                }
                catch { return null; }
            }
        }
    }
}