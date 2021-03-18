namespace BotsCore.Moduls.Translate
{
    public abstract class TranslateCore
    {
        public abstract uint MaxCharText { get; }
        public abstract Lang.LangTypes[] Langs { get; }
        public abstract string Translate(string txt, Lang.LangTypes langTo, Lang.LangTypes? lang = null);
    }
}
