using BotsCore.Bots.Interface;
using BotsCore.User;
using BotsCore.User.Models;
using System;

namespace BotsCore.Bots.Model
{
    public partial record ObjectDataMessageInBot
    {
        public delegate void RegisterUser(ObjectDataMessageInBot inBot);
        public string messageText { get; init; }
        public string callbackData { get; init; }
        public object DataMessenge { get; init; }
        public IBot botHendler { get; init; }
        public BotID botID { get; init; }
        public ModelUser User { get; private set; }
        public ModelBotUser BotUser { get; private set; }

        public void LoadInfo_User(Action<ObjectDataMessageInBot> action)
        {
            var infoUser = ManagerUser.GetUser(botID);
            if (infoUser != null)
            {
                User = infoUser.Value.user;
                BotUser = infoUser.Value.userBot;
            }
            else
            {
                User = new ModelUser();
                BotUser = new ModelBotUser(botID, new ModelBotUser.DataPage());
                User.AddModelBotUser(BotUser);
                ManagerUser.AddUser(User);
                if (action != null)
                    action.Invoke(this);
            }
        }
    }
}