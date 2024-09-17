using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private static ITelegramBotClient? client;
    private static ReceiverOptions? receiverOptions;
    private static string token = "7527295657:AAFxiTsCyYn3ROgeJNC8o3D-BFi2dD-gUKk";
    private static InlineKeyboardMarkup? keyboard;
    private static InlineKeyboardMarkup? raffle;
    private static List<long> adminIds = new List<long> { 387772149 }; // ID администраторов
    private static Dictionary<string, List<string>> raffleParticipants = new Dictionary<string, List<string>>(); // Словарь для участников розыгрыша


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
                    InlineKeyboardButton.WithCallbackData("Хочу участвовать!", "participate1"),
                }
            });

        Console.WriteLine("Бот запущен");
        Console.ReadLine();
        Console.WriteLine("Бот остановлен");
    }

    private static async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            var message = update.Message;

            if (message.Text == "/start")
            {
                Console.WriteLine($"Получено сообщение от пользователя {message.From.Username} с Id: {message.From.Id}");
                await client.SendTextMessageAsync(message.Chat.Id, $"Привет {message.From.Username}!");
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Это бот для участия в розыгрышах.\nЧтобы испытать свою удачу, выбери в каком розыгрыше ты хочешь участвовать:",
                    replyMarkup: keyboard);

                // Проверка, является ли пользователь администратором
                if (adminIds.Contains(message.From.Id))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Вы администратор. У вас есть доступ к редактированию розыгрышей.");
                    await client.SendTextMessageAsync(message.Chat.Id, "Здесь будет панель управления для администраторов.");
                }
            }
        }

        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data != null)
        {
            var callbackQuery = update.CallbackQuery;
            var user = callbackQuery.From;

            // Проверка на выбранную кнопку
            switch (callbackQuery.Data)
            {
                case "button1":
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вы выбрали: Розыгрыш 1", replyMarkup: raffle);
                    break;

                case "participate1":
                    await AddParticipantToRaffle("Розыгрыш 1", user);
                    break;
            }
        }
    }

    // Добавление пользователя в список участников
    private static async Task AddParticipantToRaffle(string raffleName, User user)
    {
        if (!raffleParticipants.ContainsKey(raffleName))
        {
            raffleParticipants[raffleName] = new List<string>();
        }

        // Проверяем, не добавлен ли пользователь уже
        if (raffleParticipants[raffleName].Contains(user.Username))
        {
            await client.SendTextMessageAsync(user.Id, $"Вы уже участвуете в {raffleName}.");
        }
        else
        {
            raffleParticipants[raffleName].Add(user.Username);
            await client.SendTextMessageAsync(user.Id, $"Вы добавлены в {raffleName}.");
        }
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
