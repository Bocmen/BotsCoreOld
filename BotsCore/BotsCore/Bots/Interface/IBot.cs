using BotsCore.Bots.Model;

namespace BotsCore.Bots.Interface
{
    public partial interface IBot
    {
        public BotTypes GetBotTypes();
        public string GetId();
        public object SendDataBot(ObjectDataMessageSend messageSend);
        public uint GetMaxLengthButtonText();
        public enum BotTypes
        {
            VK,
            Telegram
        }
    }
}
