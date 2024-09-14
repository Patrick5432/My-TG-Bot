using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static ITelegramBotClient? client;
    private static ReceiverOptions? receiverOptions;
    private static string token = "7527295657:AAFxiTsCyYn3ROgeJNC8o3D-BFi2dD-gUKk";
    private static InlineKeyboardMarkup? keyboard;
    private static InlineKeyboardMarkup? raffle;


    public static void Main(string[] args)
    {
        client = new TelegramBotClient(token);
        receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
                        {
                UpdateType.Message,
                UpdateType.CallbackQuery
            }
        };

        using var cts = new CancellationTokenSource();
        client.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);
        keyboard = new InlineKeyboardMarkup(
                                new List<InlineKeyboardButton[]>()
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Розыгрыш 1", "button1"),
                                        InlineKeyboardButton.WithCallbackData("Розыгрыш 2", "button2"),
                                        InlineKeyboardButton.WithCallbackData("Розыгрыш 3", "button3"),
                                    }
                                });
        raffle = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
               new InlineKeyboardButton[]
               {
                   InlineKeyboardButton.WithCallbackData("\bХочу участвовать!\b"),
               }
            });
        Console.WriteLine("Бот запущен");
        Console.ReadLine();
        Console.WriteLine("Бот остановлен");
    }

    private static async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                {
                    switch (update.Message.Text)
                    {
                        case "/start":
                            {
                                await client.SendTextMessageAsync(update.Message.From.Id, $"Привет {update.Message.From.Username}");
                                await client.SendTextMessageAsync(update.Message.From.Id,
                                    $"Это бот для участия в розыгрышах.\nЧтобы испытать свою удачи и выиграть приз, выбери в каком розыгрыше ты хочешь участвовать?",
                                    replyMarkup: keyboard);
                                break;
                            }
                    }
                }

                return;

            case UpdateType.CallbackQuery:
                {
                    var callbackQuery = update.CallbackQuery;
                    var user = callbackQuery.From;
                    var chat = callbackQuery.Message.Chat;
                    switch (update.CallbackQuery.Data)
                    {
                        case "button1":
                            {
                                await client.SendTextMessageAsync(chat.Id, "Вы выбрали: Розыгрыш 1", replyMarkup: raffle);
                                break;
                            }
                    }
                }

                return;

        }

    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }

}