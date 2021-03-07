using BotsCore.Moduls.Translate;

namespace BotsCore.Bots.Model.Buttons.Command.Interface
{
    public interface IGetCommandText
    {
        public string GetText(Lang.LangTypes lang);
    }
}
