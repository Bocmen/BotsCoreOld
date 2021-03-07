using System.Collections.Generic;
using static BotsCore.Moduls.Translate.Lang;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Наделяет обьект свойством перевода
    /// </summary>
    public interface ITranslate
    {
        /// <summary>
        /// Получение всех строк необходимых к переводу у обьекта
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Text> GetTextsTranslate();
        /// <summary>
        /// Перевод данных обьекта
        /// </summary>
        public void Translate(params LangTypes[] langs)
        {
            var lines = GetTextsTranslate();
            foreach (var lang in langs)
                foreach (var elem in lines)
                    elem.GetText(lang);
        }
    }
}