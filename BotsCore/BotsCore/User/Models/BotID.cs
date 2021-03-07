using BotsCore.Bots.Model;
using System;

namespace BotsCore.User.Models
{
    /// <summary>
    /// Личный иденификатор бота
    /// </summary>
    public partial class BotID
    {
        public string BotKey;
        /// <summary>
        /// Ключ пользователя у бота
        /// </summary>
        public long? Id;
        /// <summary>
        /// Тип бота
        /// </summary>
        public TypeBot bot;
        /// <summary>
        /// Типы ботов
        /// </summary>
        public enum TypeBot
        {
            Telegram,
            Vk
        }
        public static bool operator ==(BotID a, BotID b) => a?.bot == b?.bot && a?.Id == b?.Id && a.BotKey == b.BotKey;
        public static bool operator !=(BotID a, BotID b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj is BotID botID)
                return botID == this;
            return false;
        }
        public override int GetHashCode() => $"{BotKey}{Id}{bot}".GetHashCode();
        public override string ToString() => $"BotKey: {BotKey},Id: {Id},TypeBot: {bot}";
    }
}
