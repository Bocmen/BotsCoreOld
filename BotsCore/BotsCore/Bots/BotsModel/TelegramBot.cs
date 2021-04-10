using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Moduls;
using BotsCore.Moduls.GetSetting.Interface;
using BotsCore.Moduls.Translate;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using static BotsCore.Bots.Model.ObjectDataMessageSend.Media;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;
using BotsCore.Bots.Model.Buttons;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using Telegram.Bot.Types.InputFiles;
using BotsCore.Bots.Model.Buttons.Command;
using System.Text.RegularExpressions;

namespace BotsCore.Bots.BotsModel
{
    public class TelegramBot : IBot
    {
        private const int LengthText_mediaMessage = 1024;
        private const int LengthText_textMessage = 4096;
        private const int LengthText_Buttons = 32;
        public TelegramBotClient BotClient { get; init; }
        public readonly Telegram.Bot.Types.User botInfo;
        private readonly string NameAppStore;
        private const string LastMessageInfo = "LastMessageInfo";
        private const string LastKeyboardInfo = "LastKeyboardInfo";
        private static readonly Regex regexUrl = new(@"\[.*\]\(.*\)", RegexOptions.Compiled);
        private static readonly Regex regexUrlLength = new(@"\[(.*)\]\(.*\)", RegexOptions.Compiled);
        // ======================================================================================== Clear Data
        private readonly Text Text_ClearMessage = new(Lang.LangTypes.ru, "Сообщение очищено");
        private readonly Media ClearMediaData = new("http://cdn.onlinewebfonts.com/svg/img_431947.png", MediaType.Photo);

        public TelegramBot(string Token, IObjectSetting setting)
        {
            UpdateSetting(setting);
            BotClient = new TelegramBotClient(Token);
            botInfo = BotClient.GetMeAsync().Result;
            BotClient.OnMessage += Bot_OnMessage;
            BotClient.OnCallbackQuery += Bot_OnCallbackQuery;
            NameAppStore = $"Telegram Bot: {BotClient.BotId}";
            BotClient.StartReceiving();
            EchoLog.Print($"Запущен бот: {botInfo.Username}", EchoLog.PrivilegeLog.LogSystem);
        }
        /// <summary>
        /// Метод события кнопок в сообщениях
        /// </summary>
        private void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e) => Bot_InMessage(e.CallbackQuery.Message, e.CallbackQuery.Data);
        /// <summary>
        /// Метод собятия сообщений
        /// </summary>
        private void Bot_OnMessage(object sender, MessageEventArgs e) => Bot_InMessage(e.Message);
        /// <summary>
        /// Метод обработки всех событий бота
        /// </summary>
        private void Bot_InMessage(Message messageData, string CallbackQueryData = null) => ManagerPage.InMessageBot(new ObjectDataMessageInBot() { DataMessenge = messageData, MessageText = messageData.Text, CallbackData = CallbackQueryData, BotID = new User.Models.BotID() { bot = User.Models.BotID.TypeBot.Telegram, BotKey = ManagerBots.GetNameBot(this), Id = messageData.Chat.Id }, BotHendler = this });
        public IBot.BotTypes GetBotTypes() => IBot.BotTypes.Telegram;
        public string GetId() => botInfo.Id.ToString();
        /// <summary>
        /// Обновление настроек бота
        /// </summary>
        public static void UpdateSetting(IObjectSetting setting)
        {

        }
        public object SendDataBot(ObjectDataMessageSend messageSend)
        {
            Chat chatId = messageSend.InBot.DataMessenge != null ? ((Message)messageSend.InBot.DataMessenge).Chat : new Chat() { Id = (long)messageSend.InBot.BotID.Id };
            MessegeInfoOld[][] messengeOldInfo = GetOldMessage(messageSend);

            if (messageSend.ClearButtonsKeyboard)
            {
                object info = messageSend.InBot.BotUser.GetAppData<object>(NameAppStore, LastKeyboardInfo);
                if (info != null && info is MessegeInfoOld infoOldKeyboard)
                {
                    int idMessage = BotClient.SendTextMessageAsync(chatId, ".", replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton(".")) { ResizeKeyboard = true }).Result.MessageId;
                    BotClient.DeleteMessageAsync(chatId, idMessage);
                }
            }

            if (messageSend.ClearButtonsMessage)
                if (messengeOldInfo != null)
                    foreach (var group in messengeOldInfo)
                        if (group != null)
                            foreach (var item in group)
                                BotClient.EditMessageReplyMarkupAsync(chatId, item.Id, null);

            if (messageSend.ClearOldMessage)
            {
                ClearMessage(messengeOldInfo, messageSend, chatId);
                SaveOldMessageInfo(null, messageSend);
                return null;
            }
            else
            {
                if (messengeOldInfo == default || !messageSend.IsEditOldMessage)
                {
                    return SendMessageOnEdit(messageSend, chatId);
                }
                else
                {
                    if (messengeOldInfo?.Last()?.Length == 1 && (messengeOldInfo?.Last()?.Last().EditOldMessege ?? false) && (messageSend.media == null || messageSend.media.Length == 1))
                    {
                        ClearMessage(messengeOldInfo.Take(messengeOldInfo.Length - 1), messageSend, chatId);
                        MessegeInfoOld lastMessageInfo = messengeOldInfo.Last().Last();
                        if (
                            (lastMessageInfo.TypeMessage == (messageSend.media != null && messageSend.media.Length >= 1)) &&
                            ((messageSend.ButtonsKeyboard == default && messageSend.ButtonsMessage == default) ||
                            (messageSend.ButtonsKeyboard == default && messageSend.ButtonsMessage != default) ||
                            (GetLengthText(messageSend.Text) > ((lastMessageInfo.TypeMessage ? LengthText_mediaMessage : LengthText_textMessage) + (messageSend.ButtonsMessage != default ? 0 : LengthText_textMessage)))
                            )
                           )
                        {
                            MessegeInfoOld messegeSendInfo = new()
                            {
                                TypeMessage = lastMessageInfo.TypeMessage
                            };
                            if (lastMessageInfo.TypeMessage)
                            {
                                InlineKeyboardMarkup replyMarkup = IsGetMessegeButtonsSend(LengthText_mediaMessage);
                                messegeSendInfo.Id = BotClient.EditMessageMediaAsync(chatId, lastMessageInfo.Id, GetInputMediaBase(messageSend.media.First(), GetLimitText(ref messageSend, LengthText_mediaMessage)), replyMarkup: replyMarkup).Result.MessageId;
                                messageSend.media = null;
                            }
                            else
                            {
                                InlineKeyboardMarkup replyMarkup = IsGetMessegeButtonsSend(LengthText_textMessage);
                                messegeSendInfo.Id = BotClient.EditMessageTextAsync(chatId, lastMessageInfo.Id, GetLimitText(ref messageSend, LengthText_textMessage), Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: replyMarkup).Result.MessageId;
                            }

                            return SendMessageOnEdit(messageSend, chatId, new List<MessegeInfoOld[]> { new MessegeInfoOld[] { messegeSendInfo } });

                            InlineKeyboardMarkup IsGetMessegeButtonsSend(uint textLimit)
                            {
                                if (lastMessageInfo.EditOldMessege && (GetLengthText(messageSend.Text) <= textLimit))
                                {
                                    InlineKeyboardMarkup resul = GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.InBot.User.Lang);
                                    messageSend.ButtonsMessage = default;
                                    messegeSendInfo.EditOldMessege = true;
                                    return resul;
                                }
                                return default;
                            }
                        }
                        else
                        {
                            ClearMessage(new MessegeInfoOld[][] { new MessegeInfoOld[] { lastMessageInfo } }, messageSend, chatId);
                            return SendMessageOnEdit(messageSend, chatId);
                        }
                    }
                    else
                    {
                        ClearMessage(messengeOldInfo, messageSend, chatId);
                        return SendMessageOnEdit(messageSend, chatId);
                    }
                }
            }
        }
        private object SendMessageOnEdit(ObjectDataMessageSend messageSend, Chat chatId, List<MessegeInfoOld[]> sendMessageInfo = null)
        {
            if (sendMessageInfo == default)
                sendMessageInfo = new List<MessegeInfoOld[]>();

            if (string.IsNullOrWhiteSpace(messageSend.Text))
                SendRemainingTextAndButtons();

            if (messageSend.media != null && messageSend.media.Length > 1)
            {
                // Отправка фото и видео
                SendGroup(messageSend.media.Where(x => (x.Type == MediaType.Photo) || (x.Type == MediaType.Video)));
                // Отправка GIF
                SendNonGroupMedia(messageSend.media.Where(x => x.Type == MediaType.GIF));
                // Отправка документов
                SendNonGroupMedia(messageSend.media.Where(x => x.Type == MediaType.Document));
                // Отправка аудио
                SendNonGroupMedia(messageSend.media.Where(x => x.Type == MediaType.Audio));
                messageSend.media = null;

                void SendNonGroupMedia(IEnumerable<Media> media)
                {
                    if (media != null && media.Any())
                    {
                        foreach (var media_elem in media)
                        {
                            sendMessageInfo.Add(new MessegeInfoOld[]
                            {
                            new MessegeInfoOld()
                            {
                                Id = SendMedia(media_elem, null, GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang)),
                                EditOldMessege = messageSend.ButtonsKeyboard == null,
                                TypeMessage = true
                            }
                            });
                        }
                        messageSend.ButtonsKeyboard = default;
                    }
                }
            }
            // Отправка клавиатуры если встроить в текст не получается
            if (messageSend.ButtonsKeyboard != default && messageSend.ButtonsMessage != default)
            {
                if (
                    /*messageSend.IsSaveInfoMessenge || */
                    ((messageSend.media != default && messageSend.media.Length == 1) ? GetLengthText(messageSend.Text) <= LengthText_mediaMessage :
                    (GetLengthText(messageSend.Text) <= LengthText_textMessage))
                   )
                {
                    BotClient.SendTextMessageAsync
                    (
                        chatId,
                        ManagerPage.SettingManagerPage.GetTextSetButtons(messageSend.InBot),
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang)
                    );
                    messageSend.ButtonsKeyboard = null;
                }
            }

            if (messageSend.media != default && messageSend.media.Length == 1)
            {
                IReplyMarkup replyMarkup = (messageSend.ButtonsKeyboard == default ?
                    (GetLengthText(messageSend.Text) <= LengthText_mediaMessage ?
                    GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.InBot.User.Lang) : default
                    ) : GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang));
                string textSend = GetLimitText(ref messageSend, LengthText_mediaMessage);
                sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld() { Id = SendMedia(messageSend.media.First(), textSend, replyMarkup), EditOldMessege = (messageSend.ButtonsKeyboard == null), TypeMessage = true } });
                messageSend.media = default;
                messageSend.ButtonsKeyboard = default;
                if (!string.IsNullOrEmpty(messageSend.Text))
                    SendMessageOnEdit(messageSend, chatId, sendMessageInfo);
                SaveOldMessageInfo(sendMessageInfo, messageSend);
                return sendMessageInfo.ToArray();
            }
            SendRemainingTextAndButtons();
            SaveOldMessageInfo(sendMessageInfo, messageSend);
            return sendMessageInfo.ToArray();
            void SendRemainingTextAndButtons()
            {
                while (!string.IsNullOrWhiteSpace(messageSend.Text) || messageSend.ButtonsKeyboard != default || messageSend.ButtonsMessage != default)
                {
                    IReplyMarkup replyMarkup = null;
                    bool EditOldMessege = true;
                    if (messageSend.ButtonsKeyboard != default && messageSend.ButtonsMessage != default)
                    {
                        replyMarkup = GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang);
                        messageSend.ButtonsKeyboard = null;
                        EditOldMessege = false;
                    }
                    else if (messageSend.ButtonsMessage != default)
                    {
                        replyMarkup = GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.InBot.User.Lang);
                        messageSend.ButtonsMessage = null;
                    }
                    else if (messageSend.ButtonsKeyboard != default)
                    {
                        replyMarkup = GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang);
                        messageSend.ButtonsKeyboard = null;
                        EditOldMessege = false;
                    }

                    sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld()
                {
                    EditOldMessege = EditOldMessege,
                    TypeMessage = false,
                    Id = BotClient.SendTextMessageAsync
                    (
                        chatId,
                        string.IsNullOrWhiteSpace(messageSend.Text) ? ManagerPage.SettingManagerPage.GetTextSetButtons(messageSend) : GetLimitText(ref messageSend, LengthText_textMessage),
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                    ).Result.MessageId
                }});
                }
            }
            void SendGroup(IEnumerable<Media> medias)
            {
                uint count = 0;
                var MediaGroup = medias.GroupBy(x => count++ / 10).ToArray();
                foreach (var group in MediaGroup)
                {
                    if (group.Count() >= 2)
                    {
#pragma warning disable CS0618 // Тип или член устарел
                        int[] idSend = (BotClient.SendMediaGroupAsync(chatId, group.Select(x => GetInputMediaBase(x)))).Result.Select(x => x.MessageId).ToArray();
#pragma warning restore CS0618 // Тип или член устарел
                        sendMessageInfo.Add(idSend.Select(x => new MessegeInfoOld() { Id = x, EditOldMessege = false, TypeMessage = true }).ToArray());
                    }
                    else
                    {
                        sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld() { Id = SendMedia(group.Last(), null, GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.InBot.User.Lang)), EditOldMessege = (messageSend.ButtonsKeyboard == null), TypeMessage = true } });
                        messageSend.ButtonsKeyboard = default;
                    }
                }
            }
            int SendMedia(Media media, string textSend = null, IReplyMarkup replyMarkup = null)
            {
                InputOnlineFile file = new(media.Path);
                return media.Type switch
                {
                    MediaType.Photo =>
                    BotClient.SendPhotoAsync(
                        chatId,
                        file,
                        textSend,
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                        ).Result.MessageId,

                    MediaType.GIF or MediaType.Video =>
                    (BotClient.SendAnimationAsync(
                        chatId,
                        file,
                        caption: textSend,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                    )).Result.MessageId,

                    MediaType.Document =>
                    (BotClient.SendDocumentAsync(
                        chatId,
                        file,
                        caption: textSend,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                        )).Result.MessageId,

                    MediaType.Audio =>
                    (BotClient.SendAudioAsync(
                        chatId,
                        file,
                        caption: textSend,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                        )).Result.MessageId,

                    _ => -1,
                };
            }
        }
        /// <summary>
        /// Получение информации о предыдущих сообщениях
        /// </summary>
        private MessegeInfoOld[][] GetOldMessage(ObjectDataMessageSend messageSend)
        {
            if (messageSend.MessageEditObject is MessegeInfoOld[][] resul)
                return resul;
            else
            {
                string LastMessageInfo = messageSend.InBot.BotUser.GetAppData<string>(NameAppStore, TelegramBot.LastMessageInfo);
                if (LastMessageInfo != null)
                {
                    resul = JsonConvert.DeserializeObject<MessegeInfoOld[][]>(LastMessageInfo);
                    if (resul != default && resul.Length > 0)
                        return resul;
                    else
                        return null;
                }
            }
            return null;
        }
        private void SaveOldMessageInfo(IEnumerable<IEnumerable<MessegeInfoOld>> MessegeInfoOld, ObjectDataMessageSend messageSend)
        {
            messageSend.InBot.BotUser.SetAppData(NameAppStore, (LastMessageInfo, (messageSend.IsSaveInfoMessenge && MessegeInfoOld != null) ? JsonConvert.SerializeObject(MessegeInfoOld) : null));
            if (MessegeInfoOld != null)
            {
                foreach (var group in MessegeInfoOld)
                {
                    foreach (var item in group)
                    {
                        if (!item.EditOldMessege)
                        {
                            messageSend.InBot.BotUser.SetAppData(NameAppStore, (LastKeyboardInfo, item));
                        }
                    }
                }
            }
        }
        private static InputMediaBase GetInputMediaBase(Media media, string Comment = null, Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown)
        {
            InputMediaBase inputMediaBase = null;
            switch (media.Type)
            {
                case MediaType.Photo:
                    inputMediaBase = new InputMediaPhoto(media.Path);
                    break;
                case MediaType.GIF:
                    inputMediaBase = new InputMediaAnimation(media.Path);
                    break;
                case MediaType.Video:
                    inputMediaBase = new InputMediaVideo(media.Path);
                    break;
                case MediaType.Audio:
                    inputMediaBase = new InputMediaAudio(media.Path);
                    break;
                case MediaType.Document:
                    inputMediaBase = new InputMediaDocument(media.Path);
                    break;
            }
            inputMediaBase.Caption = Comment;
            inputMediaBase.ParseMode = parseMode;
            inputMediaBase.Media = inputMediaBase.Media;
            return inputMediaBase;
        }
        private void ClearMessage(IEnumerable<IEnumerable<MessegeInfoOld>> messeges, ObjectDataMessageSend messageSend, Chat chat)
        {
            if (messeges == default) return;
            string textClear = Text_ClearMessage.GetText(messageSend.InBot.User.Lang);
            foreach (var group in messeges)
            {
                foreach (var messege in group)
                {
                    if (messege.EditOldMessege)
                    {
                        try
                        {
                            BotClient.DeleteMessageAsync(chat, messege.Id);
                        }
                        catch
                        {
                            try
                            {
                                if (messege.TypeMessage)
                                {
                                    BotClient.EditMessageMediaAsync(chat, messege.Id, GetInputMediaBase(ClearMediaData));
                                }
                                else
                                    BotClient.EditMessageTextAsync(chat, messege.Id, textClear);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Клавиатура
        /// </summary>
        private static ReplyKeyboardMarkup GetReplyKeyboard(Button[][] button, Lang.LangTypes lang)
        {
            if (button == default)
                return null;
            KeyboardButton[][] buttons = button.Select(y => y.Select(x => GetButton(x)).ToArray()).ToArray();
            KeyboardButton GetButton(Button button)
            {
                //object resul = button.ButtonBot?.FirstOrDefault(x => x.typeBot == IBot.BotTypes.Telegram);
                //if (resul != default)
                //    return (KeyboardButton)resul;
                return new KeyboardButton()
                {
                    Text = ObjectCommand.FilterButtonText(button.GetNameButton(lang), LengthText_Buttons)
                };
            }
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
        /// <summary>
        /// Кнопки в сообщении
        /// </summary>
        private static InlineKeyboardMarkup GetInlineKeyboard(Button[][] button, Lang.LangTypes lang)
        {
            if (button == default)
                return null;
            InlineKeyboardButton[][] buttons = button?.Select(y => y.Select(x => GetButton(x)).ToArray()).ToArray();
            InlineKeyboardButton GetButton(Button button)
            {
                //object resul = button.ButtonBot?.FirstOrDefault(x => x.typeBot == IBot.BotTypes.Telegram);
                //if (resul != default)
                //    return (InlineKeyboardButton)resul;
                string text = ObjectCommand.FilterButtonText(button.GetNameButton(lang), LengthText_Buttons);
                return new InlineKeyboardButton()
                {
                    Text = text,
                    Url = button.Url,
                    CallbackData = text
                };
            }
            return new InlineKeyboardMarkup(buttons);
        }
        private static string GetLimitText(ref ObjectDataMessageSend messageSend, int limit)
        {
            var data = GeneratePartText(messageSend.Text);
            var replaceData = GetLimitArray(data?.Select(x => (x.Item1.First(), x.Item2)).ToArray(), limit);
            string SendText = string.Empty;
            string NotSendText = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                if (i < replaceData.item)
                {
                    SendText += data[i].Item1[data[i].Item2 ? 1 : 0];
                }
                else if (i == replaceData.item)
                {
                    if (replaceData.countChar == null)
                    {
                        SendText += data[i].Item1[data[i].Item2 ? 1 : 0];
                    }
                    else
                    {
                        SendText += data[i].Item1[0].Substring(0, (int)replaceData.countChar + 1);
                        NotSendText += data[i].Item1[0].Remove(0, (int)replaceData.countChar + 1);
                    }
                }
                else
                {
                    NotSendText += data[i].Item1[data[i].Item2 ? 1 : 0];
                }
            }
            messageSend.Text = NotSendText;
            return SendText;
            static (string[], bool)[] GeneratePartText(string text)
            {
                string[] splitArray = regexUrl.Split(text);
                string[][] matchesPart = regexUrl.Matches(text)?.Select(x => new string[] { x.Groups[0].Value.Remove(0, x.Groups[0].Value.IndexOf('[') + 1).Substring(0, x.Groups[0].Value.IndexOf(']')), x.Groups[0].Value }).ToArray();
                List<(string[], bool)> resul = new List<(string[], bool)>();
                for (int i = 0; i < splitArray.Length; i++)
                {
                    resul.Add((new string[] { splitArray[i] }, false));
                    if (i < matchesPart.Length)
                        resul.Add((matchesPart[i], true));
                }
                return resul.ToArray();
            }
            static (int item, int? countChar) GetLimitArray((string, bool)[] data, int Limit)
            {
                int len = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if ((data[i].Item1.Length + len) <= Limit)
                    {
                        len += data[i].Item1.Length;
                    }
                    else if (!data[i].Item2)
                    {
                        int NewLimit = Limit - len;
                        int indexReplace = GetLimitOrdinaryText(data[i].Item1, NewLimit);
                        if (indexReplace != 0)
                            return (i, indexReplace);
                        else
                            return (i - 1, null);
                    }
                    else
                    {
                        return (i - 1, null);
                    }
                }
                return (data.Length - 1, null);
            }
            static int GetLimitOrdinaryText(string text, int limit)
            {
                if (text.Length <= limit)
                {
                    return text.Length - 1;
                }
                char search = '\n';
            rest:
                for (int i = (int)limit - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        search = ' ';
                        goto rest;
                    }

                    if (text[i] == search)
                    {
                        return i;
                    }
                }
                return text.Length - 1;
            }
        }
        private static int GetLengthText(string text) => text != null ? regexUrlLength.Split(text).Select(x => x?.Length ?? 0).Sum() : 0;
        public uint GetMaxLengthButtonText() => LengthText_Buttons;

        public struct MessegeInfoOld
        {
            /// <summary>
            /// Идентификатор сообщения
            /// </summary>
            public int Id;
            /// <summary>
            /// Тип сообщения true - медиа, false - текстовое
            /// </summary>
            public bool TypeMessage;
            /// <summary>
            /// Можно ли отредактировать сообщение
            /// </summary>
            public bool EditOldMessege;
        }
    }
}