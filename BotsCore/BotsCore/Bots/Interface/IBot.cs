using BotsCore.Bots.Model;

namespace BotsCore.Bots.Interface
{
    public partial interface IBot
    {
        public BotTypes GetBotTypes();
        public string GetId();
        public void Start();
        public void SendDataBot(ObjectDataMessageSend messageSend);
        public enum BotTypes
        {
            VK,
            Telegram
        }
    }
}
