using BotsCore.Bots.Interface;
using System.Collections.Generic;
using BotsCore.Bots.Model;
using System.Linq;

namespace BotsCore.Bots
{
    public static class ManagerBots
    {
        private static Dictionary<string, (IBot bot, bool isStarted)> botsList = new Dictionary<string, (IBot bot, bool isStarted)>();
        public static void AddBot(IBot bot) => botsList.Add(GetNameBot(bot), (bot, false));
        public static string GetNameBot(IBot bot) => $"N={bot.GetId()}T={bot.GetBotTypes()}";
        public static void StartBots() => botsList.AsParallel().ForAll(x => { if (!x.Value.isStarted) { x.Value.bot.Start(); botsList[x.Key] = (x.Value.bot, true); } });
        public static void SendDataBot(ObjectDataMessageSend messageSend)
        {
            messageSend.inBot.botHendler.SendDataBot(messageSend);
            messageSend.inBot.User.LoadToDataBD();
        }
    }
}