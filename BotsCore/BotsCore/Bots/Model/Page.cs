using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls;
using BotsCore.User.Models;
using System;
using System.Threading.Tasks;

namespace BotsCore.Bots.Model
{
    public abstract class Page
    {
        /// <summary>
        /// Событие обработки входящих сообщений
        /// </summary>
        public abstract void EventInMessage(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Повторный запрос отправки последнего сообщения
        /// </summary>
        public abstract void ResetLastMessenge(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Событие установки страницы пользователю (первое открытие)
        /// </summary>
        public virtual void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) { }
        /// <summary>
        /// Событие загрузки страницы из хранилища (как правило вызывается после перезагрузки бота)
        /// </summary>
        /// <param name="state">true - загрузка была успешной, false - произошла ошибка и данные были утеряны</param>
        public virtual void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) { }
        /// <summary>
        /// Событие перед закрытием страницы
        /// </summary>
        public virtual void EventClose(ObjectDataMessageInBot inBot) { }
        /// <summary>
        /// Получение набора кнопок клавиатуры для данной страницы
        /// </summary>
        public virtual KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => null;
        /// <summary>
        /// Филтр отправки сообщений из другого Page при открытой данной странице
        /// </summary>
        /// <param name="messageSend">Отправляемое сообщение</param>
        public virtual ObjectDataMessageSend FilterAlienMessage(ObjectDataMessageSend messageSend, Page sendingPage) => messageSend;
        /// <summary>
        /// Фильтр отправки сообщений источник которых не известен
        /// </summary>
        /// <param name="messageSend">Отправляемые данные неизвестног источника</param>
        /// <param name="infoSource">Путь к скрипту вызвавший метод отправки</param>
        public virtual ObjectDataMessageSend FilterUnknownSenderMessage(ObjectDataMessageSend messageSend, string infoSource) => messageSend;
        /// <summary>
        /// Событие статуса отправки сообщения
        /// </summary>
        /// <param name="messageSend">Отправленное сообщение</param>
        /// <param name="status">Статус отправки true - успех, false - произошла одна или несколько ошибок</param>
        public virtual void EventStatusSendingMessage(ObjectDataMessageSend messageSend, object messageSendInfo, bool status) { }
        /// <summary>
        /// Фильтр отправляемых виджетов из других Page или неизвестных источников
        /// </summary>
        public virtual ObjectDataMessageSend FilterSetWidget(ObjectDataMessageSend messageSend, string infoSource) => messageSend;
        /// <summary>
        /// События что был отправлен виджет
        /// </summary>
        public virtual void EventSetWidget(ObjectDataMessageSend messageSend, Page sendPage, object messageSendInfo) { }
        /// <summary>
        /// Событие о том что виджет был закрыт
        /// </summary>
        public virtual void EventClosedWidget(ObjectDataMessageInBot inBot) { }
        /// <summary>
        /// Если метод GetKeyboardButtons возвращает null, выдавать ли стандартную клавиатуру
        /// </summary>
        public virtual bool IsSendStandartButtons(ObjectDataMessageInBot inBot) => true;


        /// <summary>
        /// Отправка сообщений
        /// </summary>
        /// <param name="messageSend">Данные отправляемого сообщения</param>
        /// <returns>bool - состояние отправки, object - Информация об отправленных сообщениях</returns>
        public Task<(bool, object)> SendDataBot(ObjectDataMessageSend messageSend)
        {
            return Task.Run(() =>
            {
                (object sendInfo, bool statusSend) = ManagerPage.SendDataBot(messageSend, true, this);
                EventStatusSendingMessage(messageSend, sendInfo, statusSend);
                return (statusSend, sendInfo);
            });
        }
        public string GetNamePage() => base.ToString();
        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="Text">Текст лога</param>
        /// <param name="botID">Иденификатор пользователя бота</param>
        public static void Print(string Text, BotID botID = null, EchoLog.PrivilegeLog privilegeLog = EchoLog.PrivilegeLog.Info) => EchoLog.Print(Text, $"BotsCore.Bots.Model.Page->{botID}", privilegeLog);
    }
}