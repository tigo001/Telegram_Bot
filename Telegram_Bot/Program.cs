using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("6154842902:AAHlo-7eMCVouSGfQrY8w2h00jJfSosCzCg");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            BotUpdates botUpdates = new BotUpdates(botClient);
            botUpdates.BotUpdate(update);
        }

        async static Task Error(ITelegramBotClient botClient, Exception update, CancellationToken token)
        {

        }
    }
}
