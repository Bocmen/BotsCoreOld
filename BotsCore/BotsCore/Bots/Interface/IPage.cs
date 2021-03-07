using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;

namespace BotsCore.Bots.Interface
{
    public interface IPage
    {
        public void LoadPageStore(ObjectDataMessageInBot inBot);
        public void SetPage(ObjectDataMessageInBot inBot);
        public void InMessage(ObjectDataMessageInBot inBot);
        public void ResetLastMessenge(ObjectDataMessageInBot inBot);
        public KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot);
    }
}