using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command;
using System;

namespace BotsCore.Bots.Interface
{
    public interface ISettingManagerPage
    {
        /// <summary>
        /// Стандартный набор кнопок который выдаётся по умолчанию
        /// </summary>
        public Button[][] GetStandartButtons(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Выдать ли стандартный набор кнопок при создании пользователя
        /// </summary>
        public bool SetStandartButtonsCreteUser(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Текст выводимый пользователю при его первому взаимодействию с платформой
        /// </summary>
        public string GetTextCreteUser(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Первая страница после создание пользователя
        /// </summary>
        public (string NameApp, string NamePage, object SendDataPage) GetPageCreteUser(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Стандартный текст при выдаче кнопок (если текст не был указан
        /// </summary>
        public string GetTextSetButtons(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Набор команд которые будут проверяться первыми (перед отправкой сообщения на обработку странице)
        /// </summary>
        public CommandList GetSpecialCommand(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Получения метода обрабатывающего регистрацию пользователя
        /// </summary>
        public Action<ObjectDataMessageInBot> GetRegisterMethod(ObjectDataMessageInBot inBot);
    }
}