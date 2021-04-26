using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Translate;

namespace BotsCore.Bots.Model
{
    public partial class ObjectDataMessageSend
    {
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static bool Default_EditOldMessage = true;
        public static bool Default_SaveInfoMessenge = true;
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        public ObjectDataMessageInBot InBot { get; private set; }

        private bool? saveInfoMessenge;
        private bool? editOldMessage = null;

        private string textString;

        /// <summary>
        /// Стереть старые сообщения (не совместим со всеми остальными параметрами)
        /// </summary>
        public bool ClearOldMessage;
        public bool ClearButtonsMessage;
        public bool ClearButtonsKeyboard;

        /// <summary>
        /// Отправляемый текст типа Text
        /// </summary>
        public Text TextObj { set; private get; }
        /// <summary>
        /// Отправляемый текст типа sting
        /// </summary>
        public string Text
        {
            get
            {
                if (TextObj != null)
                    return TextObj.GetText(InBot);
                else
                    return textString;
            }
            set
            {
                textString = value;
                TextObj = null;
            }
        }
        /// <summary>
        /// Медиавложения к сообщению
        /// </summary>
        public Media[] media;
        /// <summary>
        /// редактировать ли предыдущие значения
        /// </summary>
        public bool IsEditOldMessage
        {
            get
            {
                if (editOldMessage != null)
                    return (bool)editOldMessage;
                return Default_EditOldMessage;
            }
            set
            {
                editOldMessage = value;
            }
        }
        /// <summary>
        /// Является ли сообщение виджетом
        /// </summary>
        public bool Widget;
        /// <summary>
        /// Кнопки внутри сообщения
        /// </summary>
        public Button[][] ButtonsMessage;
        /// <summary>
        /// Клавиатура бота
        /// </summary>
        public Button[][] ButtonsKeyboard;
        /// <summary>
        /// Сохранить ли данные о текущем сообщении для их дальнейшего редактирования
        /// </summary>
        public bool IsSaveInfoMessenge
        {
            get
            {
                if (saveInfoMessenge != null)
                    return (bool)saveInfoMessenge;
                return Default_SaveInfoMessenge;
            }
            set
            {
                saveInfoMessenge = value;
            }
        }
        /// <summary>
        /// Обьект старых сообщений для их возможного редактирования (IsEditOldMessage в таком случае ни на что не влияет)
        /// </summary>
        public object MessageEditObject;

        public ObjectDataMessageSend(ObjectDataMessageInBot inBot) => InBot = inBot;
        public static implicit operator ObjectDataMessageInBot(ObjectDataMessageSend v) => v.InBot;

        public class Media
        {
            public Media(string path, MediaType type)
            {
                Path = path;
                Type = type;
            }
            public MediaType Type { get; private set; }
            public string Path { get; private set; }
            public enum MediaType
            {
                Photo,
                GIF,
                Document,
                Video,
                Audio
            }
        }
    }
}
