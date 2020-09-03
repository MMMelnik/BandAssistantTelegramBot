using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BandAssistantBot.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BandAssistantBot
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static TelegramBotClient _bot;

        public static async Task Main(string[] args)
        {
#if USE_PROXY
            var Proxy = new WebProxy(Configuration.Proxy.Host, Configuration.Proxy.Port) { UseDefaultCredentials = true };
            Bot = new TelegramBotClient(Configuration.BotToken, webProxy: Proxy);
#else
            _bot = new TelegramBotClient(Configuration.BotToken);
#endif

            CreateHostBuilder(args).Build().Run();

            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _bot.OnInlineQuery += BotOnInlineQueryReceived;
            _bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            _bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                // Send inline keyboard
                case "/inline":
                    await SendInlineKeyboard(message);
                    break;

                // send custom keyboard
                case "/keyboard":
                    await SendReplyKeyboard(message);
                    break;

                // send a photo
                case "/photo":
                    await SendDocument(message);
                    break;

                // request location or contact
                case "/request":
                    await RequestContactAndLocation(message);
                    break;

                default:
                    await Usage(message);
                    break;
            }

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task SendInlineKeyboard(Message message)
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                });
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard
                );
            }

            static async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new[]
                    {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                    },
                    resizeKeyboard: true
                );

                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup

                );
            }

            static async Task SendDocument(Message message)
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"data:image/png;base64,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";
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
                await _bot.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: "Nice Picture"
                );
            }

            static async Task RequestContactAndLocation(Message message)
            {
                var requestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: requestReplyKeyboard
                );
            }

            static async Task Usage(Message message)
            {
                const string usage = "Usage:\n" +
                                        "/inline   - send inline keyboard\n" +
                                        "/keyboard - send custom keyboard\n" +
                                        "/photo    - send a photo\n" +
                                        "/request  - request location or contact";
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        // Process Inline Keyboard callback data
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await _bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await _bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await _bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}
