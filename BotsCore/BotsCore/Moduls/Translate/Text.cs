using BotsCore.Bots.Model.Buttons.Command.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Обьект мультиязычного текста
    /// </summary>
    public class Text : IGetCommandText
    {
        [JsonProperty]
        private readonly List<(Lang.LangTypes lang, string text)> Data = new();

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
        public Text((Lang.LangTypes lang, string text)[] DataSet)
        {
            if (DataSet != null && DataSet.Length >= 1)
                Data.AddRange(DataSet);
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
            rest: var New = (lang, Translator.Translete(Data[0].text, lang, Data[0].lang));
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

        /// <summary>
        /// Загрузка обьекта из json
        /// </summary>
        public static Text LoadJSON(string Json)
        {
            try
            {
                dynamic data = JsonConvert.DeserializeObject(Json);
                var r = ((Newtonsoft.Json.Linq.JArray)data.Data).Select(x => x.ToObject<(Lang.LangTypes lang, string text)>()).ToArray();
                return new Text(r);
            }
            catch { throw new Model.Exception("Неудалось десериализовать JSON обьекта Text"); }
        }
        //=========================================================================================
        /// <summary>
        /// Множественный перевод нескольких обьектов (по скорости быстрее чем отдельный перевод)
        /// </summary>
        /// <param name="lang">На какой язык необходимо перевести</param>
        public static void MultiTranslate(Lang.LangTypes lang, List<Text> linesTranslate)
        {
            var LangLines = linesTranslate.Where(x => x != null && x.Data.FirstOrDefault(l => l.lang == lang) == default).GroupBy(x => x.Data[0].lang);
            foreach (var oneLang in LangLines)
                MultiTransleteOneLang(lang, oneLang.ToList());
        }
        private static void MultiTransleteOneLang(Lang.LangTypes lang, List<Text> linesTranslate)
        {
            if (linesTranslate.Sum(x => x.Data[0].text.Length) >= 4500)
            {
                MultiTransleteOneLang(lang, linesTranslate.Take(linesTranslate.Count / 2).ToList());
                MultiTransleteOneLang(lang, linesTranslate.Take(linesTranslate.Count - (linesTranslate.Count / 2)).ToList());
            }
            else
            {
                StringBuilder stringBuilder = new();
                foreach (var line in linesTranslate)
                    stringBuilder.AppendLine(line.Data[0].text);
                restErrorTranslatr: string TranslatedText = Translator.Translete(stringBuilder.ToString(), lang, linesTranslate[0].Data[0].lang);
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
    }
}