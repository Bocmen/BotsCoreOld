using BotsCore.Bots.Interface;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Moduls.Translate;
using static BotsCore.Bots.Model.Buttons.Command.ObjectCommand;

namespace BotsCore.Bots.Model.Buttons
{
    public class Button
    {
        public (IBot.BotTypes typeBot, object data)[] BotAddData;
        public string Url { get; private set; }
        public ObjectCommand ObjectCommand { get; private set; }
        public Text NameButtonObj { get; init; }
        public string NameButton { get; init; }

        /// <summary>
        /// Создание кнопки у которой её название и команды одинаковы.
        /// </summary>
        /// <param name="modelMarkerText">Название кнопки и текст события кнопки</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(Text text, InvokeCommand command)
        {
            NameButtonObj = text;
            ObjectCommand = new ObjectCommand(command, text);
        }
        /// <summary>
        /// Создание кнопки у которой её название и команды одинаковы.
        /// </summary>
        /// <param name="text">Название кнопки и текст события кнопки</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(string text, InvokeCommand command)
        {
            NameButton = text;
            ObjectCommand = new ObjectCommand(command, text);
        }
        /// <summary>
        /// Создание кнопки у которой название и команда отличаются
        /// (Применяется для callback кнопок)
        /// </summary>
        /// <param name="nameButton">Обьект с названием кнопки</param>
        /// <param name="commandText">Строка команды</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(Text nameButton, string commandText, InvokeCommand command)
        {
            NameButtonObj = nameButton;
            ObjectCommand = new ObjectCommand(command, commandText);
        }
        /// <summary>
        /// Создание кнопки у которой название и команда отличаются
        /// (Применяется для callback кнопок)
        /// </summary>
        /// <param name="nameButton">Строка с названием кнопки</param>
        /// <param name="commandText">Строка команды</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(string nameButton, string commandText, InvokeCommand command)
        {
            NameButton = nameButton;
            ObjectCommand = new ObjectCommand(command, commandText);
        }
        /// <summary>
        /// Создание кнопки уже с готовым набором команд
        /// </summary>
        /// <param name="nameButton">Обьект с названием кнопки</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(Text nameButton, ObjectCommand Ocommand)
        {
            NameButtonObj = nameButton;
            ObjectCommand = Ocommand;
        }
        /// <summary>
        /// Создание кнопки уже с готовым набором команд
        /// </summary>
        /// <param name="nameButton">Строка с названием кнопки</param>
        /// <param name="command">Команда которую нужно исполнить</param>
        public Button(string nameButton, ObjectCommand Ocommand)
        {
            NameButton = nameButton;
            ObjectCommand = Ocommand;
        }

        private Button() { }

        public string GetNameButton(Lang.LangTypes lang) => NameButton ?? NameButtonObj.GetText(lang);

        /// <summary>
        /// Создание кнопки (callback) с открытием ссылки
        /// </summary>
        /// <param name="Url">Ссылка котоая будет открыта</param>
        /// <param name="nameButton">Обьект с названиум кнопки</param>
        /// <returns></returns>
        public static Button GetButtonOpenUrl(string Url, Text nameButton) => new() { Url = Url, NameButtonObj = nameButton };
        /// <summary>
        /// Создание кнопки (callback) с открытием ссылки
        /// </summary>
        /// <param name="Url">Ссылка котоая будет открыта</param>
        /// <param name="nameButton">Строка с названием кнопки</param>
        /// <returns></returns>
        public static Button GetButtonOpenUrl(string Url, string nameButton) => new() { Url = Url, NameButton = nameButton };
    }
}
