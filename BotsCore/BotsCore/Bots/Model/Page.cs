using BotsCore.Bots.Model.Buttons;

namespace BotsCore.Bots.Model
{
    public abstract class Page
    {
        /// <summary>
        /// Событие установки страницы пользователю (первое открытие)
        /// </summary>
        public virtual void Open(ObjectDataMessageInBot inBot) { }
        /// <summary>
        /// Событие загрузки страницы из хранилища (как правило вызывается после перезагрузки бота)
        /// </summary>
        public virtual void StoreLoad(ObjectDataMessageInBot inBot) { }
        /// <summary>
        /// Событие перед закрытием страницы
        /// </summary>
        public virtual void Close(ObjectDataMessageInBot inBot) { }
        /// <summary>
        /// Событие обработки входящих сообщений
        /// </summary>
        public abstract void InMessage(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Получение набора кнопок клавиатуры для данной страницы
        /// </summary>
        public virtual KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => null;
        /// <summary>
        /// Повторный запрос отправки последнего сообщения
        /// </summary>
        public abstract void ResetLastMessenge(ObjectDataMessageInBot inBot);
    }
}