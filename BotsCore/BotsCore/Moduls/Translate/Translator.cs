using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
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
                    Lang.LangTypes.de
        };
        public override uint MaxCharText => 4000;
        public override Lang.LangTypes[] Langs => langs;

        public override string Translate(string txt, Lang.LangTypes langTo, Lang.LangTypes? lang = null)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return txt;
            using (var client = new HttpClient())
            {
                try
                {
                    string responseText = client.PostAsync("https://translate.alibaba.com/translationopenseviceapp/trans/TranslateTextAddAlignment.do", new StringContent($"srcLanguage={(lang == null ? "ru" : lang.ToString())}&tgtLanguage={langTo}&srcText={HttpUtility.UrlEncode(txt)}&viewType=&source=&bizType=message", Encoding.UTF8, "application/x-www-form-urlencoded")).Result.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrWhiteSpace(responseText)) return responseText;
                    return string.Join("\n", ((Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject<dynamic>(responseText).listTargetText).Select(x => (string)x));
                }
                catch { return null; }
            }
        }
    }
}