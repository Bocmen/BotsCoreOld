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
        public static (ModelUser user, ModelBotUser userBot)? GetUser(BotID botID)
        {
            foreach (var user in users)
                foreach (var botUser in user.BotsAccount)
                    if (botUser.BotID == botID)
                        return (user, botUser);
            return null;
        }
        public static bool AddUser(ModelUser modelUser)
        {
            foreach (var botUser in modelUser.BotsAccount)
                if (GetUser(botUser.BotID) != null)
                    return false;
            modelUser.LoadToDataBD();
            users.Add(modelUser);
            return true;
        }
        public static void DeliteUser(BotID botID)
        {
            ModelUser user = GetUser(botID).Value.user;
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
    }
}