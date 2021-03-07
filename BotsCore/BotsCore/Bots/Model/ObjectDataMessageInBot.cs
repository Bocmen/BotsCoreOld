﻿using BotsCore.Bots.Interface;
using BotsCore.User;
using BotsCore.User.Models;
using System;

namespace BotsCore.Bots.Model
{
    public partial record ObjectDataMessageInBot
    {
        public delegate void RegisterUser(ObjectDataMessageInBot inBot);
        public string MessageText { get; init; }
        public string CallbackData { get; init; }
        public object DataMessenge { get; init; }
        public IBot BotHendler { get; init; }
        public BotID BotID { get; init; }
        public ModelUser User { get; private set; }
        public ModelBotUser BotUser { get; private set; }

        public ObjectDataMessageInBot() { }

        public ObjectDataMessageInBot(ModelUser user, ModelBotUser botUser)
        {
            User = user;
            BotUser = botUser;
            BotID = botUser.BotID;
            BotHendler = ManagerBots.GetBot(BotID.BotKey);
        }

        public void LoadInfo_User(Action<ObjectDataMessageInBot> action)
        {
            var infoUser = ManagerUser.GetUser(BotID);
            if (infoUser != null)
            {
                User = infoUser.Value.user;
                BotUser = infoUser.Value.userBot;
            }
            else
            {
                User = new ModelUser();
                BotUser = new ModelBotUser(BotID, new ModelBotUser.DataPage());
                User.AddModelBotUser(BotUser);
                ManagerUser.AddUser(User);
                if (action != null)
                    action.Invoke(this);
            }
        }
        public static implicit operator BotID(ObjectDataMessageInBot v) => v.BotID;
        public static implicit operator ModelUser(ObjectDataMessageInBot v) => v.User;
        public static implicit operator ModelBotUser(ObjectDataMessageInBot v) => v.BotUser;
    }
}