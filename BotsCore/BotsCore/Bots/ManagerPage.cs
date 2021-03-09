using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using BotsCore.Moduls;
using BotsCore.User;
using BotsCore.Bots;

namespace BotsCore
{
    public static partial class ManagerPage
    {
        private const string NameAppStore = "CoreData";
        private const string NameIsWidgetStore = "IsWidget";

        /// <summary>
        /// Удалять ли старые сообщения когда приходит новое текстовое сообщение
        /// </summary>
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static bool AutoClearOldMessage = true;
        /// <summary>
        /// Обрабатывать события страницы при очистке старых сообщений или нет, true = да, false - нет
        /// </summary>
        public static bool EventsClearOldMessageOnOff = false;
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        /// <summary>
        /// Список обьектов создающих таблицы
        /// </summary>
        private static readonly List<ICreatePageApp> ListPage = new();
        /// <summary>
        /// Некоторые настройки
        /// </summary>
        public static ISettingManagerPage SettingManagerPage { get; private set; }
        /// <summary>
        /// Добавить обьект создающий страницы
        /// </summary>
        public static void Add_ICreatePageApp(ICreatePageApp createPageApp) => ListPage.Add(createPageApp);
        /// <summary>
        /// Инциализация менеджера страниц, необходимо передать его настройки
        /// </summary>
        public static void Start(ISettingManagerPage setting)
        {
            SettingManagerPage = setting;
            ManagerUser.AllEditUsers
                (x =>
                {
                    x.AllEditBotUsers(y =>
                    {
                        if (y.Page.ObjectPage is not Page)
                        {
                            y.Page.ObjectPage = GetPageUser(new ObjectDataMessageInBot(x, y));
                        }
                        return y;
                    });
                    return x;
                });
        }
        /// <summary>
        /// Получение открытой страницы у пользователя
        /// </summary>
        public static Page GetPageUser(ObjectDataMessageInBot inBot)
        {
            if (inBot.BotUser.Page.ObjectPage == default)
            {
                (string NameApp, string NamePage) = SettingManagerPage.GetPageCreteUser(inBot);
                inBot.BotUser.Page.ObjectPage = GetPage(NameApp, NamePage, inBot.BotHendler.GetBotTypes(), inBot.BotHendler.GetId());
                return (Page)inBot.BotUser.Page.ObjectPage;
            }
            if (inBot.BotUser.Page.ObjectPage is Page page)
            {
                return page;
            }
            else
            {
                foreach (var elem in ListPage)
                {
                    if (elem.GetNameApp() == inBot.BotUser.Page.NameApp)
                    {
                        Page PageUser = null;
                        bool state = false;
                        try
                        {
                            object objPage = JsonConvert.DeserializeObject(inBot.BotUser.Page.ObjectPage.ToString(), elem.GetTypePage(inBot.BotUser.Page.NamePage));
                            if (objPage is Page pageDeserialize)
                            {
                                PageUser = pageDeserialize;
                                state = true;
                            }
                        }
                        catch
                        {
                            PageUser = (Page)GetPage(inBot.BotUser.Page.NameApp, inBot.BotUser.Page.NamePage, inBot.BotHendler.GetBotTypes(), inBot.BotHendler.GetId());
                        }
                        try { PageUser.EventStoreLoad(inBot, state); } catch (Exception e) { EchoLog.Print($"Не удалось обработать метод загрузки данных страницы из бд, лог оишибки: {e.Message}"); }
                        break;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Получить экземпляр страницы
        /// </summary>
        /// <param name="appName">Название подраздела</param>
        /// <param name="namePage">Название страницы</param>
        /// <returns></returns>
        private static object GetPage(string appName, string namePage, IBot.BotTypes? botType = null, string keyBot = null)
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
            var pageO = GetPage(NameApp, NamePage, inBot.BotHendler.GetBotTypes(), inBot.BotHendler.GetId());
            if (pageO != null && pageO is Page)
            {
                if (inBot.BotUser.Page.ObjectPage is Page pageOpen)
                    pageOpen.EventClose(inBot);
                inBot.BotUser.Page = new User.Models.ModelBotUser.DataPage() { NameApp = NameApp, NamePage = NamePage, ObjectPage = pageO };
                ((Page)inBot.BotUser.Page.ObjectPage).EventOpen(inBot);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Обработка входящих сообщений
        /// </summary>
        public static void InMessageBot(ObjectDataMessageInBot inBot)
        {
            if (inBot.LoadInfo_User(SettingManagerPage.GetRegisterMethod(inBot)))
            {
                var (NameApp, NamePage) = SettingManagerPage.GetPageCreteUser(inBot);
                SetPageSaveHistory(inBot, NameApp, NamePage);
                string SendText = SettingManagerPage.GetTextCreteUser(inBot);
                if (SettingManagerPage.SetStandartButtonsCreteUser(inBot))
                {
                    SendText ??= SettingManagerPage.GetTextSetButtons(inBot);
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = SendText, ButtonsKeyboard = SettingManagerPage.GetStandartButtons(inBot), IsSaveInfoMessenge = false }, false);
                }
                else if(!string.IsNullOrWhiteSpace(SendText))
                {
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = SendText, IsSaveInfoMessenge = false }, false);
                }
            }
            if (AutoClearOldMessage && string.IsNullOrWhiteSpace(inBot.CallbackData))
                SendDataBot(new ObjectDataMessageSend(inBot) { ClearOldMessage = true }, EventsClearOldMessageOnOff);

            try { GetPageUser(inBot).EventInMessage(inBot); inBot.User.LoadToDataBD(); } catch (Exception e) { EchoLog.Print($"Не удалось обработать сообщение, лог ошибки: {e.Message}"); }
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
            if (SetPage(inBot, NameApp, NamePage))
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
            if (GetPage(appName, namePage, inBot.BotHendler.GetBotTypes(), inBot.BotHendler.GetId()) != null)
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
            if (inBot.BotUser.GetAppData<bool>(NameAppStore, NameIsWidgetStore))
            {
                GetPageUser(inBot).ResetLastMessenge(inBot);
                inBot.BotUser.SetAppData(NameAppStore, (NameIsWidgetStore, false));
                return true;
            }
            else
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
            }
        }
        private static void AddHistryPageNonCheckManagerPage(ObjectDataMessageInBot inBot, string appName, string namePage)
        {
            if (inBot.BotUser.HistoryPage.Count >= 5)
                inBot.BotUser.HistoryPage.RemoveFirst();
            inBot.BotUser.HistoryPage.AddLast((appName, namePage));
        }
        /// <summary>
        /// Отправка данных пользователю бота
        /// </summary>
        /// <param name="messageSend">Данные для отправки</param>
        /// <param name="sendPage">Откуда данные отправляются, null - неизвестный источник, обьект Page указывает с какой страницы производилась отправка</param>
        /// <returns></returns>
        public static (object messageSendInfo, bool statusSend) SendDataBot(ObjectDataMessageSend messageSend, bool eventOnOff, Page sendPage = null)
        {
            try
            {
                object messageSendInfo;
                Page pageSetUser = GetPageUser(messageSend.InBot);
                messageSend.InBot.BotUser.SetAppData(NameAppStore, (NameIsWidgetStore, messageSend.Widget));
                IBot botSend = messageSend.InBot.BotHendler != default ? messageSend.InBot.BotHendler : ManagerBots.GetBot(messageSend.InBot.BotID.BotKey);
                if (eventOnOff)
                {
                    string infoSource = sendPage != null ? sendPage.GetNamePage() : new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.FullName.ToString();
                    if (messageSend.Widget)
                    {
                        messageSendInfo = botSend.SendDataBot(sendPage == pageSetUser ? messageSend : pageSetUser.FilterSetWidget(messageSend, infoSource));
                        pageSetUser.EventSetWidget(messageSend, sendPage, messageSendInfo);
                    }
                    else
                    {
                        if (sendPage != null)
                        {
                            if (pageSetUser != sendPage)
                                messageSendInfo = botSend.SendDataBot(pageSetUser.FilterAlienMessage(messageSend));
                            else
                                messageSendInfo = botSend.SendDataBot(messageSend);
                        }
                        else
                            messageSendInfo = botSend.SendDataBot(pageSetUser.FilterUnknownSenderMessage(messageSend, infoSource));
                    }
                }
                else
                    messageSendInfo = botSend.SendDataBot(messageSend);

                messageSend.InBot.User.LoadToDataBD();
                return (messageSendInfo, true);
            }
            catch (Exception e)
            {
                EchoLog.Print($"Не удалось выполнить отправку сообщений. [{e.Message}]", EchoLog.PrivilegeLog.Warning);
            }
            return (null, false);
        }
    }
}