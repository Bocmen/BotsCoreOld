using BotsCore.Bots.Model.Buttons;

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

        public bool ClearOldMessage;
        public string Text;
        public Media[] media;
        public bool IsEditOldMessage
        {
            get
            {
                if (editOldMessage != null)
                    return (bool)editOldMessage;
                return Default_EditOldMessage;
            }
            init
            {
                editOldMessage = value;
            }
        }
        public bool Widget;
        public Button[][] ButtonsMessage;
        public Button[][] ButtonsKeyboard;
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
        public object MessageEditObject;

        public ObjectDataMessageSend(ObjectDataMessageInBot inBot) => this.InBot = inBot;

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
