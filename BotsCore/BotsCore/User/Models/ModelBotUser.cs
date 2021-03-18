using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BotsCore.User.Models
{
    public partial class ModelBotUser
    {
        /// <summary>
        /// Идентификатор бота
        /// </summary>
        public BotID BotID { get; protected set; }
        public DataPage Page;
        public LinkedList<(string NameApp, string NamePage, object dataInPage)> HistoryPage = new();
        /// <summary>
        /// Данные микросервисов
        /// </summary>
        public readonly Dictionary<string, Dictionary<string, object>> appData = new();

        public object this[string nameApp, string namePage]
        {
            get
            {
                return GetAppData<object>(nameApp, namePage);
            }
            set
            {
                SetAppData(nameApp, (namePage, value));
            }
        }

        public ModelBotUser(BotID botID, DataPage Page)
        {
            BotID = botID;
            this.Page = Page;
        }

        protected ModelBotUser() { }

        //================================================================ Взаимодействие с данными микросервиса
        /// <summary>
        /// Получение данных для микросервиса
        /// </summary>
        public T GetAppData<T>(string NameApp, string Key)
        {
            var resul = appData.GetValueOrDefault(NameApp)?.GetValueOrDefault(Key);
            if (resul is T resulReturn)
                return resulReturn;
            return default;
        }
        /// <summary>
        /// Изменение данных микросервиса
        /// </summary>
        /// <param name="setData">(string Ключ, object записываемые_данные)[]</param>
        public void SetAppData(string NameApp, params (string KeyData, object SetData)[] setData)
        {
            if (!appData.ContainsKey(NameApp))
                appData.Add(NameApp, new Dictionary<string, object>());
            var appDictonary = appData.GetValueOrDefault(NameApp);
            foreach (var (KeyData, SetData) in setData)
            {
                if (appDictonary.ContainsKey(KeyData))
                    appDictonary[KeyData] = SetData;
                else
                    appDictonary.Add(KeyData, SetData);
            }
        }
        public struct DataPage
        {
            public string NameApp;
            public string NamePage;
            public object ObjectPage;
        }
    }
}
