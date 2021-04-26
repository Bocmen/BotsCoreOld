using BotsCore.User.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using BotsCore.Moduls;
using BotsCore.Moduls.GetSetting.Interface;
using System;
using System.Linq;

namespace BotsCore.User
{
    public static class ManagerUser
    {

        private static List<ModelUser> users = new();
        public static string PatchTableUsersInfo { get; private set; }
        public static string BdConnectionUsersInfo { get; private set; }

        public static void Start(IObjectSetting setting)
        {
            //  File.Delete(Path.GetFullPath(setting.GetValue("patchTableUsersInfo")));
            PatchTableUsersInfo = setting.GetValue("patchTableUsersInfo");
            BdConnectionUsersInfo = string.Format(setting.GetValue("bdConnectionUsersInfo"), PatchTableUsersInfo);
            if (Directory.Exists(PatchTableUsersInfo))
                Directory.CreateDirectory(PatchTableUsersInfo);
            if (File.Exists(PatchTableUsersInfo))
            {
                using SQLiteConnection connection = new(BdConnectionUsersInfo);
                connection.Open();
                SQLiteDataReader reader = (new SQLiteCommand("SELECT * FROM 'Users';", connection)).ExecuteReader();
                while (reader.Read())
                    users.Add(ModelUser.LoadInJson(reader.GetString(1), reader.GetInt64(0)));
                reader.CloseAsync();
                connection.CloseAsync();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(PatchTableUsersInfo)));
                SQLiteConnection.CreateFile(PatchTableUsersInfo);
                using (SQLiteConnection connection = new(BdConnectionUsersInfo))
                {
                    connection.Open();
                    new SQLiteCommand("CREATE TABLE \"Users\" (\"Id\" INTEGER PRIMARY KEY, \"Data\" TEXT);", connection).ExecuteNonQuery();
                    connection.CloseAsync();
                }
                EchoLog.Print("Создана таблица бд пользователей");
            }
        }
        public static (ModelUser user, ModelBotUser userBot)? GetFirstUserBotID(BotID botID)
        {
            foreach (var user in users)
                foreach (var botUser in user.BotsAccount)
                    if (botUser.BotID == botID)
                        return (user, botUser);
            return null;
        }
        public static ModelBotUser GetFirstBotUser(Func<ModelBotUser, bool> serachF)
        {
            foreach (var user in users)
                foreach (var botUser in user.BotsAccount)
                    if (serachF.Invoke(botUser))
                        return botUser;
            return null;
        }
        public static ModelUser GetFirstUser(Func<ModelUser, bool> serachF)
        {
            foreach (var user in users)
                if (serachF.Invoke(user))
                    return user;
            return null;
        }
        public static ModelBotUser[] GetBotUsers(Func<ModelBotUser, bool> serachF)
        {
            List<ModelBotUser> resul = new();
            foreach (var user in users)
                foreach (var botUser in user.BotsAccount)
                    if (serachF.Invoke(botUser))
                        resul.Add(botUser);
            if (resul.Any())
                return resul.ToArray();
            return null;
        }
        public static ModelUser[] GetUsers(Func<ModelUser, bool> serachF)
        {
            List<ModelUser> resul = new();
            foreach (var user in users)
                if (serachF.Invoke(user))
                    resul.Add(user);
            if (resul.Any())
                return resul.ToArray();
            return null;
        }
        public static bool AddUser(ModelUser modelUser)
        {
            foreach (var botUser in modelUser.BotsAccount)
                if (GetFirstUserBotID(botUser.BotID) != null)
                    return false;
            modelUser.LoadToDataBD();
            users.Add(modelUser);
            return true;
        }
        public static void DeliteUser(BotID botID)
        {
            ModelUser user = GetFirstUserBotID(botID).Value.user;
            user.DeliteUserDataBD();
            users.Remove(user);
        }
        public static void DeliteUser(ModelUser user)
        {
            if (users.Contains(user))
            {
                user.DeliteUserDataBD();
                users.Remove(user);
            }
        }
        public static void AllEditUsers(Func<ModelUser, ModelUser> editFunction)
        {
            lock (users) { users = users.Select(x => editFunction.Invoke(x)).ToList(); }
        }
        public static ModelUser SearchUser(Func<ModelUser, bool> searchFunction)
        {
            foreach (var user in users)
                if (searchFunction.Invoke(user))
                    return user;
            return null;
        }
    }
}