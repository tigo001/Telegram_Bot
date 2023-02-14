using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram_Bot.Models;

namespace Telegram_Bot
{
    internal class BotUpdates
    {
        ITelegramBotClient BotClient { get; set; }
        static List<QuestionModel> questions;
        #region Constructors
        public BotUpdates()
        {

        }

        public BotUpdates(ITelegramBotClient client)
        {
            BotClient = client;
        }
        #endregion

        public void BotUpdate(Update update)
        {
            switch(update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    RespondToMessage(update);
                    break;

                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    RespondToMessageCommand(update);
                    break;

                default:
                    break;
            }
        }

        private async void RespondToMessage(Update update)
        {
            var message = update.Message;
            if (message == null)
                return;

            Console.WriteLine($"From {message.From.Username ?? "Unknown user"}: {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    await BotClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Hello. I'm a victorina bot, here you can check some tests.", replyMarkup: KeyboardFunctions.GetFastKeysOnStart());
                    break;

                default:
                    break;
            }
        }

        private async void RespondToMessageCommand(Update update)
        {
            var query = update.CallbackQuery;
            if (query == null)
                return;

            Console.WriteLine($"From {query.From.Username ?? "Unknown user"}: COMMAND// {query.Data}");

            switch(query.Data)
            {
                case "Choose test":
                    await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "Choose topic.", replyMarkup: KeyboardFunctions.GetTopics());
                    break;

                case "History Of Armenia":
                case "Math":
                case "Physics":
                    await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "Ok, let's start.");
                    questions = Global.GetDataFromJson(query.Data.Replace(" ", string.Empty));
                    await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: questions[0].Number + ". " + questions[0].Question, replyMarkup: KeyboardFunctions.GetAnswers(questions[0]));
                    break;

                default:
                    
                    break;
            }
        }

        private async void GiveNextQuestion(Update update)
        {

        }
    }
}
