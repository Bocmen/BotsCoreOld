using BotsCore.Bots.Model.Buttons.Command.Interface;
using System;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons.Command
{
    public class ObjectCommand
    {
        public delegate bool InvokeCommand(ObjectDataMessageInBot inBotData, double degreeSimilarity, object data = null);
        public InvokeCommand InvokeMethod { get; init; }
        public object[] Commands { private get; init; }

        public ObjectCommand(InvokeCommand method, params object[] commands)
        {
            if (commands.FirstOrDefault(x => !(x is string || x is IGetCommandText)) != default)
                throw new Exception("Не все команды подходят для работы с данным элементов");
            Commands = commands.Select(x => { if (x is string str) { return FilterText(str); } else { return x; } }).ToArray();
            InvokeMethod = method;
        }
        public bool IsSame(ObjectDataMessageInBot messageInBot)
        {
            string textCommand = messageInBot.CallbackData ?? messageInBot.MessageText;
            textCommand = textCommand != null ? FilterText(textCommand) : null;
            foreach (var elemCommand in Commands)
            {
                if (elemCommand is string lineCommand)
                {
                    if (lineCommand == textCommand)
                        return true;
                }
                else if (elemCommand is IGetCommandText IGetCommand)
                {
                    if (textCommand == FilterText(IGetCommand.GetText(messageInBot.User.Lang)))
                        return true;
                }
            }
            return false;
        }
        private static string FilterText(string text) => string.Join(" ", text.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))).ToLower();
    }
}