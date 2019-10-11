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
        readonly string TelegramApiToken;

        public GoogleSpeechUkrBot(string TelegramApiToken)
        {
            this.TelegramApiToken = TelegramApiToken;

            var Bot = new TelegramBotClient(TelegramApiToken);

            Bot.SetWebhookAsync("");

            Bot.OnInlineQuery += async (object updobj, InlineQueryEventArgs iqea) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(iqea.InlineQuery.Query) || string.IsNullOrWhiteSpace(iqea.InlineQuery.Query))
                        return;

                    var url = new GoogleTTS.gTTS(iqea.InlineQuery.Query, URLonly: true).URL;

                    if (!string.IsNullOrEmpty(url))
                    {
                        var inline = new InlineQueryResultVoice[1];

                        inline[0] = new InlineQueryResultVoice("0", url, iqea.InlineQuery.Query);

                        await Bot.AnswerInlineQueryAsync(iqea.InlineQuery.Id, inline);
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            };

            Bot.OnMessage += async (object updobj, MessageEventArgs mea) =>
            {
                var message = mea.Message;

                if (message.Type == MessageType.Voice)
                {
                    try
                    {
                        var gSTT = new GoogleSTT.gSTT($"https://api.telegram.org/file/bot{TelegramApiToken}/{Bot.GetFileAsync(message.Voice.FileId).Result.FilePath}", 48000);
                        await Bot.SendTextMessageAsync(message.Chat.Id, gSTT.Result, replyToMessageId: message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Не вдалося розпізнати повідомлення.", replyToMessageId: message.MessageId);
                    }
                }
                else if (mea.Message.Type == MessageType.Text)
                {
                    if (message.Text == null)
                        return;

                    var ChatId = message.Chat.Id;

                    string command = message.Text.ToLower().Replace("@googlespeechukrbot", "").Replace("/", "");

                    switch (command)
                    {
                        case "start":
                            await Bot.SendTextMessageAsync(ChatId, "Вітаю! Я @GoogleSpeechUkrBot!\nНадішліть мені текстове повідомлення, щоб синтезувати мовлення, або надішліть голосове повідомлення, щоб розпізнати мовлення.\nНатисніть '/', щоби обрати команду.");
                            break;

                        case "sendvoice":
                            await Bot.SendTextMessageAsync(ChatId, "Натисніть кнопку та оберіть чат до якого хочете надіслати фразу.", replyMarkup: new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithSwitchInlineQuery("Надіслати") }));
                            break;

                        default:
                            await Bot.SendVoiceAsync(ChatId, new InputFileStream(new MemoryStream(new GoogleTTS.gTTS(command).ToByteArray())).Content, replyToMessageId: message.MessageId);
                            break;
                    }
                }
            };

            Bot.StartReceiving();
        }
    }
}