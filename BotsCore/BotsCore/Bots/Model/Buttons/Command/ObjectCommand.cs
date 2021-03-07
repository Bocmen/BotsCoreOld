using BotsCore.Bots.Model.Buttons.Command.Interface;
using System;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons.Command
{
    public class ObjectCommand
    {
        public delegate void InvokeCommand(ObjectDataMessageInBot inBotData, object data = null);
        public InvokeCommand invokeMethod { get; init; }
        public object[] commands { private get; init; }

        public ObjectCommand(InvokeCommand method, params object[] commands)
        {
            if (commands.FirstOrDefault(x => !(x.GetType() == typeof(string) || x.GetType() == typeof(IGetCommandText))) != default)
                throw new Exception("Не все команды подходят для работы с данным элементов");
            this.commands = commands;
            invokeMethod = method;
        }
        public bool IsSame(ObjectDataMessageInBot messageInBot)
        {
            string textCommand = messageInBot.messageText;
            textCommand = textCommand != null ? textCommand.Replace("  ", " ").ToLower() : null;
            foreach (var elemCommand in commands)
            {
                if (elemCommand is string lineCommand)
                {
                    if (lineCommand == textCommand)
                        return true;
                }else if (elemCommand is IGetCommandText IGetCommand)
                {
                    if (textCommand == IGetCommand.GetText(messageInBot.User.Lang))
                        return true;
                }
            }
            return false;
        }
    }
}