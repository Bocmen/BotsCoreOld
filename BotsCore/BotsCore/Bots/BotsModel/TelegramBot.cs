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

namespace BotsCore.Bots.BotsModel
{
    public class TelegramBot : IBot
    {
        private const uint LengthText_mediaMessage = 1024;
        private const uint LengthText_textMessage = 4096;
        public TelegramBotClient BotClient { get; init; }
        public readonly Telegram.Bot.Types.User botInfo;
        private readonly string NameAppStore;
        private const string LastMessageInfo = "LastMessageInfo";
        // ======================================================================================== Clear Data
        private readonly Text Text_ClearMessage = new Text(Lang.LangTypes.ru, "Сообщение очищено");
        private readonly Media ClearMediaData = new Media("http://cdn.onlinewebfonts.com/svg/img_431947.png", MediaType.Photo);

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
        private void Bot_InMessage(Message messageData, string CallbackQueryData = null) => ManagerPage.InMessageBot(new ObjectDataMessageInBot() { DataMessenge = messageData, messageText = messageData.Text, callbackData = CallbackQueryData, botID = new User.Models.BotID() { bot = User.Models.BotID.TypeBot.Telegram, BotKey = ManagerBots.GetNameBot(this), Id = messageData.Chat.Id }, botHendler = this });
        public IBot.BotTypes GetBotTypes() => IBot.BotTypes.Telegram;
        public string GetId() => botInfo.Id.ToString();
        /// <summary>
        /// Обновление настроек бота
        /// </summary>
        public static void UpdateSetting(IObjectSetting setting)
        {

        }
        public void SendDataBot(ObjectDataMessageSend messageSend)
        {
            Chat chatId = messageSend.inBot.DataMessenge != null ? ((Message)messageSend.inBot.DataMessenge).Chat : new Chat() { Id = (long)messageSend.inBot.botID.Id };
            MessegeInfoOld[][] messengeOldInfo = GetOldMessage(messageSend);
            if (messageSend.ClearOldMessage)
            {
                ClearMessage(messengeOldInfo, messageSend);
                SaveOldMessageInfo(null, messageSend);
                return;
            }
            else
            {
                if (messengeOldInfo == default || messageSend.IsEditOldMessage == false)
                {
                    SendMessageOnEdit(messageSend, chatId);
                }
                else
                {
                    if (messengeOldInfo?.Last().Length == 1)
                    {
                        ClearMessage(messengeOldInfo.Take(messengeOldInfo.Length - 1), messageSend);
                        MessegeInfoOld lastMessageInfo = messengeOldInfo.Last().Last();
                        if (
                            (lastMessageInfo.TypeMessage == (messageSend.media != null && messageSend.media.Length >= 1)) &&
                            ((messageSend.ButtonsKeyboard == default && messageSend.ButtonsMessage == default) || ((messageSend.ButtonsKeyboard == default && messageSend.ButtonsMessage != default) ? lastMessageInfo.SetMessageButton || (messageSend.Text.Length > (lastMessageInfo.TypeMessage ? LengthText_mediaMessage : LengthText_textMessage)) :
                            !messageSend.IsSaveInfoMessenge && (messageSend.Text.Length > ((lastMessageInfo.TypeMessage ? LengthText_mediaMessage : LengthText_textMessage) + (messageSend.ButtonsMessage != default ? 0 : LengthText_textMessage)))
                            )
                            )
                           )
                        {
                            MessegeInfoOld messegeSendInfo = new MessegeInfoOld
                            {
                                TypeMessage = lastMessageInfo.TypeMessage
                            };
                            if (lastMessageInfo.TypeMessage)
                            {
                                InlineKeyboardMarkup replyMarkup = IsGetMessegeButtonsSend(LengthText_mediaMessage);
                                messegeSendInfo.Id = BotClient.EditMessageMediaAsync(chatId, lastMessageInfo.Id, GetInputMediaBase(messageSend.media.First(), GetLimitText(ref messageSend.Text, LengthText_mediaMessage)), replyMarkup: replyMarkup).Result.MessageId;
                                messageSend.media = null;
                            }
                            else
                            {
                                InlineKeyboardMarkup replyMarkup = IsGetMessegeButtonsSend(LengthText_textMessage);
                                messegeSendInfo.Id = BotClient.EditMessageTextAsync(chatId, lastMessageInfo.Id, GetLimitText(ref messageSend.Text, LengthText_textMessage), Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: replyMarkup).Result.MessageId;
                            }

                            SendMessageOnEdit(messageSend, chatId, new List<MessegeInfoOld[]> { new MessegeInfoOld[] { messegeSendInfo } });

                            InlineKeyboardMarkup IsGetMessegeButtonsSend(uint textLimit)
                            {
                                if (lastMessageInfo.SetMessageButton && (messageSend.Text.Length <= textLimit))
                                {
                                    InlineKeyboardMarkup resul = GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.inBot.User.Lang);
                                    messageSend.ButtonsMessage = default;
                                    messegeSendInfo.SetMessageButton = true;
                                    return resul;
                                }
                                return default;
                            }
                        }
                        else
                        {
                            ClearMessage(new MessegeInfoOld[][] { new MessegeInfoOld[] { lastMessageInfo } }, messageSend);
                            SendMessageOnEdit(messageSend, chatId);
                        }
                    }
                    else
                    {
                        ClearMessage(messengeOldInfo, messageSend);
                        SendMessageOnEdit(messageSend, chatId);
                    }
                }
            }
        }
        private void SendMessageOnEdit(ObjectDataMessageSend messageSend, Chat chatId, List<MessegeInfoOld[]> sendMessageInfo = null)
        {
            if (sendMessageInfo == default)
                sendMessageInfo = new List<MessegeInfoOld[]>();

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
                    foreach (var media_elem in media)
                    {
                        sendMessageInfo.Add(new MessegeInfoOld[]
                        {
                            new MessegeInfoOld()
                            {
                                Id = SendMedia(media_elem, null, GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang)),
                                SetMessageButton = true,
                                TypeMessage = true
                            }
                        });
                    }
                    messageSend.ButtonsKeyboard = default;
                }
            }

            // Отправка клавиатуры если встроить в текст не получается
            if (messageSend.ButtonsKeyboard != default && messageSend.ButtonsMessage != default)
            {
                if (
                     messageSend.IsSaveInfoMessenge || ((messageSend.media != default && messageSend.media.Length == 1) ? messageSend.Text.Length <= LengthText_mediaMessage :
                    (messageSend.Text.Length <= LengthText_textMessage))
                   )
                {
                    BotClient.SendTextMessageAsync
                    (
                        chatId,
                        ManagerPage.settingManagerPage.GetTextSetButtons(messageSend.inBot.User.Lang),
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang)
                    );
                    messageSend.ButtonsKeyboard = null;
                }
            }

            if (messageSend.media != default && messageSend.media.Length == 1)
            {
                IReplyMarkup replyMarkup = (messageSend.ButtonsKeyboard == default ?
                    (messageSend.Text.Length <= LengthText_mediaMessage ?
                    GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.inBot.User.Lang) : default
                    ) : GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang));
                string textSend = GetLimitText(ref messageSend.Text, LengthText_mediaMessage);
                sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld() { Id = SendMedia(messageSend.media.First(), textSend, replyMarkup), SetMessageButton = true, TypeMessage = true } });
                messageSend.media = default;
                messageSend.ButtonsKeyboard = default;
                if (!string.IsNullOrEmpty(messageSend.Text))
                    SendMessageOnEdit(messageSend, chatId, sendMessageInfo);
                SaveOldMessageInfo(sendMessageInfo, messageSend);
                return;
            }
            else if (!string.IsNullOrWhiteSpace(messageSend.Text))
            {
            resendMessage:
                IReplyMarkup replyMarkup = null;
                bool SetMessageButton = true;
                if (messageSend.ButtonsKeyboard != default && messageSend.ButtonsMessage != default)
                {
                    replyMarkup = GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang);
                    messageSend.ButtonsKeyboard = null;
                    SetMessageButton = false;
                }
                else if (messageSend.ButtonsMessage != default)
                {
                    replyMarkup = GetInlineKeyboard(messageSend.ButtonsMessage, messageSend.inBot.User.Lang);
                    messageSend.ButtonsMessage = null;
                }
                else if (messageSend.ButtonsKeyboard != default)
                {
                    replyMarkup = GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang);
                    messageSend.ButtonsKeyboard = null;
                }

                sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld()
                {
                    SetMessageButton = SetMessageButton,
                    TypeMessage = false,
                    Id = BotClient.SendTextMessageAsync
                    (
                        chatId,
                        GetLimitText(ref messageSend.Text, LengthText_textMessage),
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                    ).Result.MessageId
                }});

                if (!string.IsNullOrWhiteSpace(messageSend.Text))
                    goto resendMessage;
                SaveOldMessageInfo(sendMessageInfo, messageSend);
                return;
            }
            SaveOldMessageInfo(sendMessageInfo, messageSend);
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
                        sendMessageInfo.Add(idSend.Select(x => new MessegeInfoOld() { Id = x, SetMessageButton = false, TypeMessage = true }).ToArray());
                    }
                    else
                    {
                        sendMessageInfo.Add(new MessegeInfoOld[] { new MessegeInfoOld() { Id = SendMedia(group.Last(), null, GetReplyKeyboard(messageSend.ButtonsKeyboard, messageSend.inBot.User.Lang)), SetMessageButton = true, TypeMessage = true } });
                        messageSend.ButtonsKeyboard = default;
                    }
                }
            }
            int SendMedia(Media media, string textSend = null, IReplyMarkup replyMarkup = null)
            {
                InputOnlineFile file = new InputOnlineFile(media.Path);
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
            string LastMessageInfo = messageSend.inBot.BotUser.GetAppData<string>(NameAppStore, TelegramBot.LastMessageInfo);
            if (LastMessageInfo != null)
            {
                MessegeInfoOld[][] resul = JsonConvert.DeserializeObject<MessegeInfoOld[][]>(LastMessageInfo);
                if (resul != default && resul.Length > 0)
                    return resul;
                else
                    return null;
            }
            return null;
        }
        private void SaveOldMessageInfo(IEnumerable<IEnumerable<MessegeInfoOld>> messegeInfoOlds, ObjectDataMessageSend messageSend)
        {
            if (messageSend.IsSaveInfoMessenge)
                messageSend.inBot.BotUser.SetAppData(NameAppStore, (TelegramBot.LastMessageInfo, messageSend.IsSaveInfoMessenge ? JsonConvert.SerializeObject(messegeInfoOlds) : null));
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
        private void ClearMessage(IEnumerable<IEnumerable<MessegeInfoOld>> messeges, ObjectDataMessageSend messageSend)
        {
            if (messeges == default) return;
            Chat chat = ((Message)messageSend.inBot.DataMessenge).Chat;
            string textClear = Text_ClearMessage.GetText(messageSend.inBot.User.Lang);
            foreach (var group in messeges)
            {
                foreach (var messege in group)
                {
                    if (messege.TypeMessage)
                    {
                        BotClient.EditMessageMediaAsync(chat, messege.Id, GetInputMediaBase(ClearMediaData));
                    }
                    else
                        BotClient.EditMessageTextAsync(chat, messege.Id, textClear); ;
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
                object resul = button.ButtonBot?.FirstOrDefault(x => x.typeBot == IBot.BotTypes.Telegram);
                if (resul != default)
                    return (KeyboardButton)resul;
                return new KeyboardButton()
                {
                    Text = button.NameButton.GetText(lang)
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
                object resul = button.ButtonBot?.FirstOrDefault(x => x.typeBot == IBot.BotTypes.Telegram);
                if (resul != default)
                    return (InlineKeyboardButton)resul;
                return new InlineKeyboardButton()
                {
                    Text = button.NameButton.GetText(lang),
                    Url = button.Url,
                    CallbackData = button.NameButton.GetText(lang)
                };
            }
            return new InlineKeyboardMarkup(buttons);
        }
        private static string GetLimitText(ref string text, uint limit)
        {
            string resul;
            if (text.Length <= limit)
            {
                resul = text;
                text = string.Empty;
                return resul;
            }
            for (int i = (int)limit - 1; i > 0; i--)
            {
                if (text[i] == ' ')
                {
                    resul = text.Substring(0, i + 1);
                    text = text.Remove(0, i + 1);
                    return resul;
                }
            }
            resul = text;
            text = null;
            return resul;
        }
        private struct MessegeInfoOld
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
            /// Можно путём редактирования встроить кнопки в сообщения
            /// </summary>
            public bool SetMessageButton;
        }
    }
}