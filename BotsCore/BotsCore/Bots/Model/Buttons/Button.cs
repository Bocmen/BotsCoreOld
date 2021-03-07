using BotsCore.Bots.Interface;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Bots.Model.Buttons.Command.Interface;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using static BotsCore.Bots.Model.Buttons.Command.ObjectCommand;

namespace BotsCore.Bots.Model.Buttons
{
    public class Button
    {
        public string Url { get; private set; }
        public (IBot.BotTypes typeBot, object data)[] ButtonBot { get; init; }
        public (IBot.BotTypes typeBot, object data)[] BotAddData { get; init; }
        public ObjectCommand objectCommand { get; private set; }
        public Text NameButton { get; init; }
        public Button(ModelMarkerTextData modelMarkerText, InvokeCommand command)
        {
            objectCommand = new ObjectCommand(command, (IGetCommandText)modelMarkerText);
        }
        public Button(string text, InvokeCommand command)
        {
            NameButton = new Text(Lang.LangTypes.ru, text);
            objectCommand = new ObjectCommand(command, text);
        }
        public Button(ObjectCommand command)
        {
            objectCommand = command;
        }
    }
}
