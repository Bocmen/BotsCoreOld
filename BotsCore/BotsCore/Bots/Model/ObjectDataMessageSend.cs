using BotsCore.Bots.Model.Buttons;

namespace BotsCore.Bots.Model
{
    public partial class ObjectDataMessageSend
    {
        public static bool Default_EditOldMessage = true;
        public static bool Default_AddBuffer = true;
        public static bool Default_SaveInfoMessenge = true;

        public ObjectDataMessageInBot inBot { get; private set; }

        public bool EditOldMessageClear { get; init; }
        public string Text;
        public Media[] media;
        private bool? editOldMessage = null;
        public bool EditOldMessage
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
        public bool? addBuffer = null;
        public bool AddBuffer
        {
            get
            {
                if (addBuffer != null)
                    return (bool)addBuffer;
                return Default_AddBuffer;
            }
            init
            {
                addBuffer = value;
            }
        }
        public bool Widget { get; init; }
        public Button[][] ButtonsMessage;
        public Button[][] ButtonsKeyboard;
        private bool? saveInfoMessenge;
        public bool SaveInfoMessenge
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

        public ObjectDataMessageSend(ObjectDataMessageInBot inBot) => this.inBot = inBot;

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
