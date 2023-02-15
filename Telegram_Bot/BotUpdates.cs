using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram_Bot.Models;
using System.Collections;

namespace Telegram_Bot
{
    internal class BotUpdates
    {
        ITelegramBotClient BotClient { get; set; }
        static List<QuestionModel> questions;
        static QuestionModel currentQuestion = null;
        static int currentScore = 0;
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
                    await BotClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Hello. I'm a quiz bot, here you can check some tests.", replyMarkup: KeyboardFunctions.GetFastKeysOnStart());
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
                    ResetQuiz(query);
                    await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: currentQuestion.Number + ". " + currentQuestion.Question, replyMarkup: KeyboardFunctions.GetAnswers(currentQuestion));
                    break;

                default:
                    if(CheckAnswer(query.Data.Substring(0, 1)))
                    {
                        await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "You're right.");
                        currentScore++;
                    }
                    else
                        await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "Right answer is " + GetAnswer(currentQuestion.Answer));
                    GiveNextQuestion(query);
                    break;
            }
        }

        private async void GiveNextQuestion(CallbackQuery query)
        {
            int index = Global.GetQuestionIndex(query.Message.Text);
            if (index < 10)
            {
                currentQuestion = questions[index];
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: currentQuestion.Number + ". " + currentQuestion.Question, replyMarkup: KeyboardFunctions.GetAnswers(currentQuestion));
            }
            else
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: $"You passed the quiz. Your score: {currentScore}");
        }

        private void ResetQuiz(CallbackQuery query)
        {
            questions = Global.GetDataFromJson(query.Data.Replace(" ", string.Empty));
            currentQuestion = questions[0];
            currentScore = 0;
        }

        private bool CheckAnswer(string answer)
        {
            if(currentQuestion == null)
                return false;

            int rightAnswer = currentQuestion.Answer;
            if ((rightAnswer == 1 && answer == "A") ||
                (rightAnswer == 2 && answer == "B") ||
                (rightAnswer == 3 && answer == "C") ||
                (rightAnswer == 4 && answer == "D"))
                return true;

            return false;
        }

        private string GetAnswer(int answer)
        {
            switch (answer)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                default:
                    return "";
            }
        }
    }
}
