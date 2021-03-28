using BotsCore.Bots.Model.Buttons.Command.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Обьект мультиязычного текста
    /// </summary>
    public class Text : IGetCommandText
    {
        [NonSerialized]
        public static TranslateCore translateCore = new Translator();
        [JsonProperty]
        private readonly List<(Lang.LangTypes lang, string text)> Data = new();
        [JsonProperty]
        public bool LockTranslator = false;

        /// <summary>
        /// Инцилизация при известном одном языке
        /// </summary>
        public Text(Lang.LangTypes lang, string Text)
        {
            Data.Add((lang, Text));
        }
        /// <summary>
        /// Инцилизация при известных нескольких языках
        /// </summary>
        [JsonConstructor]
        public Text((Lang.LangTypes lang, string text)[] Data)
        {
            if (Data != null && Data.Length >= 1)
                this.Data.AddRange(Data);
            else
                throw new ArgumentNullException();
        }
        /// <summary>
        /// Получить текст на заданном языке (в случае отсутствия данных осуществляется автоперевод)
        /// </summary>
        public string GetText(Lang.LangTypes lang)
        {
            var resul = Data.FirstOrDefault(x => x.lang == lang);
            if (resul != default)
                return resul.text;
            if (LockTranslator)
                return Data.First().text;
            rest: var New = (lang, translateCore.Translate(Data[0].text, lang, Data[0].lang));
            if (New.Item2 == null) goto rest;
            if (Data.FirstOrDefault(x => x.lang == lang) == default)
                Data.Add(New);
            return New.Item2;
        }
        /// <summary>
        /// Список буферезированных языков
        /// </summary>
        public Lang.LangTypes[] GetLangsTranslated() => Data.Select(x => x.lang).ToArray();
        /// <summary>
        /// Получение всех сохраненных данных
        /// </summary>
        public (Lang.LangTypes lang, string text)[] GetAllData() => Data.ToArray();

        public override string ToString() => $"{string.Join(",", Data.Select(x => x.lang))} text={Data[0].text}";
        public string GetDefaultText() => Data[0].text;

        public static bool operator ==(Text t1, Text t2) => t1?.GetDefaultText() == t2?.GetDefaultText();
        public static bool operator !=(Text t1, Text t2) => !(t1 == t2);
        public static bool operator ==(Text t1, object t2)
        {
            if (t2 is Text T2)
                return t1 == T2;
            return false;
        }
        public static bool operator !=(Text t1, object t2) => !(t1 == t2);


        //=========================================================================================
        /// <summary>
        /// Множественный перевод нескольких обьектов (по скорости быстрее чем отдельный перевод)
        /// </summary>
        /// <param name="lang">На какой язык необходимо перевести</param>
        public static void MultiTranslate(Lang.LangTypes lang, IEnumerable<Text> linesTranslate)
        {
            var LangLines = linesTranslate?.Where(x => x != null && (!x.LockTranslator) && x.Data.FirstOrDefault(l => l.lang == lang) == default)?.GroupBy(x => x.Data[0].lang);
            if (!LangLines.Any()) return;
            foreach (var oneLang in LangLines)
                MultiTransleteOneLang(lang, oneLang.ToList());
        }
        private static void MultiTransleteOneLang(Lang.LangTypes lang, List<Text> linesTranslate)
        {
            if (linesTranslate.Sum(x => x.Data[0].text.Length) >= translateCore.MaxCharText)
            {
                MultiTransleteOneLang(lang, linesTranslate.Take(linesTranslate.Count / 2).ToList());
                MultiTransleteOneLang(lang, linesTranslate.Skip(linesTranslate.Count / 2).ToList());
            }
            else
            {
            restErrorTranslatr: string TranslatedText = translateCore.Translate(string.Join("\n", linesTranslate.Select(x => x.Data[0].text)), lang, linesTranslate[0].Data[0].lang);
                if (TranslatedText == null) goto restErrorTranslatr;
                string[] TranslatedLinesText = TranslatedText.Split('\n');
                List<string> arrayDataAllLines = new();
                int[] CountLines = linesTranslate.Select(x => x.Data[0].text.Split('\n').Length).ToArray();
                int CountChar = 0;
                foreach (var count in CountLines)
                {
                    arrayDataAllLines.Add(string.Join("\n", TranslatedLinesText.Skip(CountChar).Take(count)));
                    CountChar += count;
                }
                for (int i = 0; i < linesTranslate.Count; i++)
                {
                    if (linesTranslate[i].Data.FirstOrDefault(x => x.lang == lang) == default)
                        linesTranslate[i].Data.Add((lang, arrayDataAllLines[i]));
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Text text)
                return text == this;
            return false;
        }

        public override int GetHashCode() => GetDefaultText().GetHashCode();
    }
}