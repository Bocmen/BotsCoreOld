using System.Collections.Generic;
using static BotsCore.Moduls.Translate.Lang;
using System.IO;
using System.Data.SQLite;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BotsCore.User.Models
{
    public partial class ModelUser
    {
        private bool IsDelite = false;
        private long? IdTable;

        /// <summary>
        /// Язык пользователя
        /// </summary>
        private LangTypes lang = LangTypes.ru;
        /// <summary>
        /// Язык пользователя (при изменении данные автоматически сохраняются)
        /// </summary>
        public LangTypes Lang
        {
            get
            {
                return lang;
            }
            set
            {
                lang = value;
            }
        }

        /// <summary>
        /// Данные о акк ботов
        /// </summary>
        public List<ModelBotUser> BotsAccount { get; private set; } = new List<ModelBotUser>();
        public Dictionary<string, object> DopInfoUser = new Dictionary<string, object>();
        public string Login;
        public string Password;
        public object this[string key]
        {
            get
            {
                if (DopInfoUser.TryGetValue(key, out object resul))
                    return resul;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    if (DopInfoUser.ContainsKey(key))
                        DopInfoUser[key] = value;
                    else
                        DopInfoUser.Add(key, value);
                }
                else
                {
                    if (DopInfoUser.ContainsKey(key))
                        DopInfoUser.Remove(key);
                }
            }
        }

        /// <summary>
        /// Удаление акк. бота
        /// </summary>
        /// <param name="id">Личный индификатор акк.</param>
        public void DeliteUserBot(BotID BotID)
        {
            for (int i = 0; i < BotsAccount.Count; i++)
            {
                if (BotsAccount[i].BotID == BotID)
                {
                    BotsAccount.Remove(BotsAccount[i]);
                    return;
                }
            }
        }
        /// <summary>
        /// Получения обьекта бота
        /// </summary>
        public ModelBotUser GetModelBotUser(BotID BotID)
        {
            foreach (var userBot in BotsAccount)
            {
                if (userBot.BotID == BotID)
                {
                    return userBot;
                }
            }
            return null;
        }
        /// <summary>
        /// Добавления обьекта бота
        /// </summary>
        /// <param name="botUser"></param>
        public void AddModelBotUser(ModelBotUser botUser)
        {
            if (GetModelBotUser(botUser.BotID) == null)
                BotsAccount.Add(botUser);
        }
        public void LoadToDataBD()
        {
            if (!IsDelite)
            {
                if (!File.Exists(ManagerUser.PatchTableUsersInfo))
                    SQLiteConnection.CreateFile(ManagerUser.PatchTableUsersInfo);
                using SQLiteConnection connection = new SQLiteConnection(ManagerUser.BdConnectionUsersInfo);
                connection.Open();
                if (IdTable == null)
                {
                    new SQLiteCommand($"INSERT INTO 'Users' ('Data') VALUES('{JsonConvert.SerializeObject(this)}');", connection).ExecuteNonQuery();
                    IdTable = connection.LastInsertRowId;
                }
                else
                    new SQLiteCommand($"UPDATE 'Users' SET Data = '{JsonConvert.SerializeObject(this)}'  WHERE Id = '{IdTable}'", connection).ExecuteNonQuery();
                connection.CloseAsync();
            }
        }
        public void DeliteUserDataBD()
        {
            if (File.Exists(ManagerUser.PatchTableUsersInfo) && IdTable != null)
            {
                using SQLiteConnection connection = new SQLiteConnection(ManagerUser.BdConnectionUsersInfo);
                connection.Open();
                new SQLiteCommand($"DELETE FROM 'Users' WHERE Id = '{IdTable}';", connection).ExecuteNonQuery();
                connection.CloseAsync();
                IsDelite = true;
            }
        }

        public static ModelUser LoadInJson(string json, long IdTable)
        {
            ModelUser resul = JsonConvert.DeserializeObject<ModelUser>(json);
            resul.IdTable = IdTable;
            return resul;
        }

        public void AllEditBotUsers(Func<ModelBotUser, ModelBotUser> editFunction)
        {
            lock (BotsAccount) { BotsAccount = BotsAccount.Select(x => editFunction.Invoke(x)).ToList(); }
        }
    }
}