using BotsCore.Bots.Interface;
using System.Collections.Generic;
using BotsCore.Bots.Model;
using System.Linq;
using BotsCore.User.Models;
using System;
using BotsCore.Moduls;

namespace BotsCore.Bots
{
    public static class ManagerBots
    {
        private static readonly Dictionary<string, IBot> botsList = new();
        public static void AddBot(IBot bot) => botsList.Add(GetNameBot(bot), bot);
        public static string GetNameBot(IBot bot) => $"N={bot.GetId()}T={bot.GetBotTypes()}";
        public static (object messageSendInfo, bool statusSend) SendDataBot(ObjectDataMessageSend messageSend, Page sendPage = null)
        {
            try
            {
                IBot botSend = messageSend.InBot.BotHendler != default ? messageSend.InBot.BotHendler : GetBot(messageSend.InBot.BotID.BotKey);
                if (botSend != default)
                {
                    object data = botSend.SendDataBot(messageSend);
                    return (data, true);
                }
            }
            catch (Exception e)
            {
                EchoLog.Print($"Не удалось выполнить отправку сообщений. [{e.Message}]", EchoLog.PrivilegeLog.Warning);
            }
            return (null, false);
        }
        public static IBot GetBot(string key)
        {
            if (botsList.TryGetValue(key, out IBot resul))
                return resul;
            return default;
        }
    }
}