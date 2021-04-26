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
            Telegram,
            Bot_1,
            Bot_2,
            Bot_3,
            Bot_4,
            Bot_5,
            Bot_6,
            Bot_7,
            Bot_8,
            Bot_9,
            Bot_10
        }
    }
}
