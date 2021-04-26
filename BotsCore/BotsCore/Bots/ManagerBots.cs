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
        private static readonly Dictionary<string, IBot> botsList = new Dictionary<string, IBot>();
        public static void AddBot(IBot bot) => botsList.Add(GetNameBot(bot), bot);
        public static string GetNameBot(IBot bot) => $"N={bot.GetId()}T={bot.GetBotTypes()}";
        public static IBot GetBot(string key)
        {
            if (botsList.TryGetValue(key, out IBot resul))
                return resul;
            return default;
        }
    }
}