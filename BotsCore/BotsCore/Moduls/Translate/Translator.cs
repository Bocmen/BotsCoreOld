using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Модуль перевода текста
    /// </summary>
    public static class Translator
    {
        /// <summary>
        /// Перевод текста
        /// </summary>
        /// <param name="langTo">Язык на который текст будет переведён</param>
        /// <param name="lang">Исходный язык (необязательный параметр)</param>
        public static string Translete(string txt, Lang.LangTypes langTo, Lang.LangTypes? lang = null)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return txt;
            using (var client = new HttpClient())
            {
                try
                {
                    string responseText = client.PostAsync("https://translate.alibaba.com/translationopenseviceapp/trans/TranslateTextAddAlignment.do", new StringContent($"srcLanguage={(lang == null ? "ru" : lang.ToString())}&srcText={txt}&tgtLanguage={langTo}&viewType=&source=&bizType=message", Encoding.UTF8, "application/x-www-form-urlencoded")).Result.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrWhiteSpace(responseText)) return responseText;
                    return string.Join("\n", ((Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject<dynamic>(responseText).listTargetText).Select(x => (string)x));

                }
                catch { return null; }
            }
        }
    }
}