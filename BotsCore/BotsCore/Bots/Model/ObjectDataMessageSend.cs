using BotsCore.Bots.Model.Buttons;

namespace BotsCore.Bots.Model
{
    public partial class ObjectDataMessageSend
    {
        public static bool Default_EditOldMessage = true;
        public static bool Default_SaveInfoMessenge = true;

        public ObjectDataMessageInBot inBot { get; private set; }

        private bool? saveInfoMessenge;
        private bool? editOldMessage = null;

        public bool ClearOldMessage { get; init; }
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
        public bool Widget { get; init; }
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
