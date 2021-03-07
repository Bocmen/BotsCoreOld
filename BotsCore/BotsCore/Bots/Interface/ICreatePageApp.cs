using System;

namespace BotsCore.Bots.Interface
{
    public interface ICreatePageApp
    {
        public string GetNameApp();
        public object GetPage(string name, IBot.BotTypes? botType = null, string keyBot = null);
        public Type GetTypePage(string name, IBot.BotTypes? botType = null, string keyBot = null);
    }
}