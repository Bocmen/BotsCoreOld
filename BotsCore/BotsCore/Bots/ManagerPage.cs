using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using BotsCore.Moduls;

namespace BotsCore
{
    public static partial class ManagerPage
    {
        /// <summary>
        /// Список обьектов создающих таблицы
        /// </summary>
        private static List<ICreatePageApp> ListPage = new List<ICreatePageApp>();
        /// <summary>
        /// Некоторые настройки
        /// </summary>
        public static ISettingManagerPage settingManagerPage { get; private set; }
        /// <summary>
        /// Добавить обьект создающий страницы
        /// </summary>
        public static void Add_ICreatePageApp(ICreatePageApp createPageApp) => ListPage.Add(createPageApp);
        /// <summary>
        /// Инциализация менеджера страниц, необходимо передать его настройки
        /// </summary>
        public static void Start(ISettingManagerPage setting) => settingManagerPage = setting;
        /// <summary>
        /// Получить экземпляр страницы
        /// </summary>
        /// <param name="appName">Название подраздела</param>
        /// <param name="namePage">Название страницы</param>
        /// <returns></returns>
        public static object GetPage(string appName, string namePage, IBot.BotTypes? botType = null, string keyBot = null)
        {
            foreach (var item in ListPage)
                if (item.GetNameApp() == appName)
                    return item.GetPage(namePage, botType, keyBot);
            return null;
        }
        /// <summary>
        /// Устанивить страницу пользователю
        /// </summary>
        /// <param name="inBot">Входящие данные от бота</param>
        /// <param name="NameApp">Название подраздела</param>
        /// <param name="NamePage">Название страницы</param>
        public static bool SetPage(ObjectDataMessageInBot inBot, string NameApp, string NamePage)
        {
            var pageO = GetPage(NameApp, NamePage, inBot.botHendler.GetBotTypes(), inBot.botHendler.GetId());
            if (pageO != null && pageO is IPage page)
            {
                inBot.BotUser.Page = new User.Models.ModelBotUser.DataPage() { NameApp = NameApp, NamePage = NamePage, ObjectPage = pageO };
                ((IPage)inBot.BotUser.Page.ObjectPage).SetPage(inBot);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Обработка входящих сообщений
        /// </summary>
        public static void InMessageBot(ObjectDataMessageInBot inBot)
        {
            inBot.LoadInfo_User(settingManagerPage.GetRegisterMethod());
            if (inBot.BotUser.Page.ObjectPage == default)
            {
                var PageSetInfo = settingManagerPage.GetPageCreteUser();
                inBot.BotUser.Page.ObjectPage = GetPage(PageSetInfo.NameApp, PageSetInfo.NamePage, inBot.botHendler.GetBotTypes(), inBot.botHendler.GetId());
            }
            if (inBot.BotUser.Page.ObjectPage is IPage page)
            {
                try { page.InMessage(inBot); } catch (Exception e) { EchoLog.Print($"Не удалось обработать сообщение, лог ошибки: {e.Message}"); }
            }
            else
            {
                foreach (var elem in ListPage)
                {
                    if (elem.GetNameApp() == inBot.BotUser.Page.NameApp)
                    {
                        inBot.BotUser.Page.ObjectPage = JsonConvert.DeserializeObject(inBot.BotUser.Page.ObjectPage.ToString(), elem.GetTypePage(inBot.BotUser.Page.NamePage));
                        if (inBot.BotUser.Page.ObjectPage is IPage pageDeserialize)
                        {
                            try { pageDeserialize.LoadPageStore(inBot); } catch (Exception e) { EchoLog.Print($"Не удалось обработать метод загрузки данных страницы из бд, лог оишибки: {e.Message}"); }
                            InMessageBot(inBot);
                        }
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Установить страницу пользователю и сохранить в истории
        /// </summary>
        /// <param name="inBot">Входящие даннные от бота</param>
        /// <param name="NameApp">Название подраздела</param>
        /// <param name="NamePage">Название страницы</param>
        /// <returns>true- страница найдена и установлена, false - произошла ошибка при установке страницы</returns>
        public static bool SetPageSaveHistory(ObjectDataMessageInBot inBot, string NameApp, string NamePage)
        {
            if (BotsCore.ManagerPage.SetPage(inBot, NameApp, NamePage))
            {
                AddHistryPageNonCheckManagerPage(inBot, NameApp, NamePage);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Запомнить страницу в истории
        /// </summary>
        /// <param name="inBot">Входящие даннные от бота</param>
        /// <param name="appName">Название подраздела</param>
        /// <param name="namePage">Название страницы</param>
        /// <returns>true- страница найдена и записана в историю, false - произошла ошибка</returns>
        public static bool AddHistoryPage(ObjectDataMessageInBot inBot, string appName, string namePage)
        {
            if (GetPage(appName, namePage, inBot.botHendler.GetBotTypes(), inBot.botHendler.GetId()) != null)
            {
                AddHistryPageNonCheckManagerPage(inBot, appName, namePage);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Возвращает на предыдущию страницу
        /// </summary>
        /// <param name="inBot">Входящие даннные от бота</param>
        /// <returns>true - успех, false -  в истории больше не осталось страниц</returns>
        public static bool SetBackPage(ObjectDataMessageInBot inBot)
        {
            inBot.BotUser.HistoryPage.RemoveLast();
            if (inBot.BotUser.HistoryPage.Count > 1)
            {
                var pageSetInfo = inBot.BotUser.HistoryPage.Last;
                inBot.BotUser.HistoryPage.RemoveLast();
                SetPageSaveHistory(inBot, pageSetInfo.Value.NameApp, pageSetInfo.Value.NamePage);
                return true;
            }
            return false;
            //else
            //{
            //    inBot.BotUser.HistoryPage.RemoveLast();
            //    SetPageSaveHistory
            //    (
            //        inBot,
            //        CreatePageAppStandart.NameApp,
            //        inBot.User.ModelRegisterId.IsRegister ? CreatePageAppStandart.NamePage_Main : CreatePageAppStandart.NamePage_RegisterMain
            //    );
            //}
        }

        private static void AddHistryPageNonCheckManagerPage(ObjectDataMessageInBot inBot, string appName, string namePage)
        {
            if (inBot.BotUser.HistoryPage.Count >= 5)
                inBot.BotUser.HistoryPage.RemoveFirst();
            inBot.BotUser.HistoryPage.AddLast((appName, namePage));
        }
    }
}