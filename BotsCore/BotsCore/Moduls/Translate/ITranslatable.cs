using System.Collections.Generic;
using static BotsCore.Moduls.Translate.Lang;

namespace BotsCore.Moduls.Translate
{
    /// <summary>
    /// Наделяет обьект свойством перевода
    /// </summary>
    public interface ITranslatable
    {
        /// <summary>
        /// Получение всех строк необходимых к переводу у обьекта
        /// </summary>
        /// <returns></returns>
        public List<Text> GetTextsTranslate();
        /// <summary>
        /// Перевод данных обьекта
        /// </summary>
        public virtual void Translate(params LangTypes[] langs)
        {
            var lines = GetTextsTranslate();
            if (langs != null)
                foreach (var lang in langs)
                    Text.MultiTranslate(lang, lines);
        }
    }
}