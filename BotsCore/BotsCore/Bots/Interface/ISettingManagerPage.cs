using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Moduls.Translate;
using System;

namespace BotsCore.Bots.Interface
{
    public interface ISettingManagerPage
    {
        public Button[][] GetStandartButtons();
        public bool SetStandartButtonsCreteUser();
        public string GetTextCreteUser(Lang.LangTypes lang);
        public (string NameApp, string NamePage) GetPageCreteUser();
        public string GetTextSetButtons(Lang.LangTypes lang);
        public CommandList GetSpecialCommand();
        public Action<ObjectDataMessageInBot> GetRegisterMethod();
    }
}