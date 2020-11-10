using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;

namespace GoogleSpeechUkrBot
{
    public class GoogleSpeechUkrBot
    {
        private TelegramBotClient bot;

        public void Start() => bot.StartReceiving();

        public void Stop() => bot.StopReceiving();

        public GoogleSpeechUkrBot(string TelegramApiToken, string ConnectionString)
        {
            var userSettings = new UserSettings(ConnectionString);

            bot = new TelegramBotClient(TelegramApiToken);

            bot.SetWebhookAsync("");

            bot.OnInlineQuery += async (object updobj, InlineQueryEventArgs iqea) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(iqea.InlineQuery.Query) || string.IsNullOrWhiteSpace(iqea.InlineQuery.Query))
                        return;

                    var url = new GoogleTTS.gTTS(iqea.InlineQuery.Query, URLonly: true).URL;

                    if (!string.IsNullOrEmpty(url))
                    {
                        var inline = new InlineQueryResultVoice[]
                        {
                            new InlineQueryResultVoice("0", url, iqea.InlineQuery.Query)
                        };

                        await bot.AnswerInlineQueryAsync(iqea.InlineQuery.Id, inline);
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            };

            bot.OnMessage += async (object updobj, MessageEventArgs mea) =>
            {
                var message = mea.Message;

                if (message.Type == MessageType.Voice && userSettings.GetVoice(message.Chat.Id))
                {
                    try
                    {
                        var gSTT = new GoogleSTT.gSTT($"https://api.telegram.org/file/bot{TelegramApiToken}/{bot.GetFileAsync(message.Voice.FileId).Result.FilePath}");
                        await bot.SendTextMessageAsync(message.Chat.Id, gSTT.Result, replyToMessageId: message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Не вдалося розпізнати повідомлення. Помилка: {ex.Message}", replyToMessageId: message.MessageId);
                    }
                }
                else if (mea.Message.Type == MessageType.Text)
                {
                    if (message.Text == null)
                        return;

                    var ChatId = message.Chat.Id;

                    var command = message.Text.ToLower().Replace("@googlespeechukrbot", "").Replace("/", "");

                    switch (command)
                    {
                        case "start":
                            await bot.SendTextMessageAsync(ChatId, "Вітаю! Я @GoogleSpeechUkrBot!\nНадішліть мені текстове повідомлення, щоб синтезувати голосове повідомлення, або надішліть голосове повідомлення, щоб розпізнати мовлення.\nНатисніть '/', щоби обрати команду.");
                            break;

                        case "sendvoice":
                            await bot.SendTextMessageAsync(ChatId, "Оберіть чат, до якого хочете надіслати голосове повідомлення.", replyMarkup: new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithSwitchInlineQuery("Надіслати") }));
                            break;

                        case "voiceon":
                            userSettings.ManageVoice(ChatId, true);
                            await bot.SendTextMessageAsync(message.Chat.Id, "Увімкнено розпізнавання мовлення у цьому чаті.", replyToMessageId: message.MessageId);
                            break;

                        case "voiceoff":
                            userSettings.ManageVoice(ChatId, false);
                            await bot.SendTextMessageAsync(message.Chat.Id, "Вимкнено розпізнавання мовлення у цьому чаті.", replyToMessageId: message.MessageId);
                            break;

                        case "texton":
                            userSettings.ManageText(ChatId, true);
                            await bot.SendTextMessageAsync(message.Chat.Id, "Увімкнено синтез мовлення у цьому чаті.", replyToMessageId: message.MessageId);
                            break;

                        case "textoff":
                            userSettings.ManageText(ChatId, false);
                            await bot.SendTextMessageAsync(message.Chat.Id, "Вимкнено синтез мовлення у цьому чаті.", replyToMessageId: message.MessageId);
                            break;

                        default:
                            if (userSettings.GetText(message.Chat.Id))
                            {
                                try
                                {
                                    await bot.SendVoiceAsync(ChatId, new InputFileStream(new MemoryStream(new GoogleTTS.gTTS(command).ToByteArray())).Content, replyToMessageId: message.MessageId);
                                }
                                catch
                                {
                                    await bot.SendTextMessageAsync(message.Chat.Id, "Під час синтезу мовлення виникла помилка.", replyToMessageId: message.MessageId);
                                }
                            }
                            break;
                    }
                }
            };
        }
    }
}