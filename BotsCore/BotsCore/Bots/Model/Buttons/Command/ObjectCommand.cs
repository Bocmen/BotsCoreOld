using BotsCore.Bots.Model.Buttons.Command.Interface;
using System;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons.Command
{
    public class ObjectCommand
    {
        public delegate bool InvokeCommand(ObjectDataMessageInBot inBotData, double degreeSimilarity, object data = null);
        public InvokeCommand InvokeMethod { get; private set; }
        private object[] Commands;

        public ObjectCommand(InvokeCommand method, params object[] commands)
        {
            if (commands.FirstOrDefault(x => !(x is string || x is IGetCommandText)) != default)
                throw new Exception("Не все команды подходят для работы с данным элементов");
            Commands = commands; // commands.Select(x => { if (x is string str) { return FilterText(str); } else { return x; } }).ToArray();
            InvokeMethod = method;
        }
        public bool IsSame(ObjectDataMessageInBot messageInBot)
        {
            string textCommand = messageInBot.CallbackData ?? messageInBot.MessageText;
            textCommand = textCommand != null ? FilterText(textCommand, messageInBot) : null;
            foreach (var elemCommand in Commands)
            {
                if (elemCommand is string lineCommand)
                {
                    if (FilterText(lineCommand, messageInBot) == textCommand)
                        return true;
                }
                else if (elemCommand is IGetCommandText IGetCommand)
                {
                    if (textCommand == FilterText(IGetCommand.GetText(messageInBot.User.Lang), messageInBot))
                        return true;
                }
            }
            return false;
        }
        private static string FilterText(string text, ObjectDataMessageInBot inBot) => FilterButtonText(string.Join(" ", text.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))).ToLower(), inBot.BotHendler.GetMaxLengthButtonText());
        public static string FilterButtonText(string text, uint maxSize)
        {
            if (text.Length > maxSize)
                text = $"{new string(text.Take((int)(maxSize - 4)).ToArray())} ...";
            return text;
        }
    }
}