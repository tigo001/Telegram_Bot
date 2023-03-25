using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram_Bot.Models;
using System.Collections;
using Telegram.Bot.Types.Enums;
using System.Linq;
using static Telegram_Bot.BotEnums;

namespace Telegram_Bot
{
    internal class BotUpdates
    {
        ITelegramBotClient BotClient { get; set; }
        static Dictionary<User, List<QuestionModel>> questionsByUsers;
        static Dictionary<User, QuestionModel> currentQuestionByUsers;
        static Dictionary<User, int> currentScoreByUsers;
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
                    await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: currentQuestionByUsers[query.From].Number + ". " + currentQuestionByUsers[query.From].Question, replyMarkup: KeyboardFunctions.GetAnswers(currentQuestionByUsers[query.From]));
                    break;

                default:
                    if(CheckAnswer(query))
                    {
                        await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "You're right.");
                        currentScoreByUsers[query.From]++;
                    }
                    else
                        await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: "Right answer is " + GetAnswer(currentQuestionByUsers[query.From].Answer));
                    GiveNextQuestion(query);
                    break;
            }
        }

        private async void GiveNextQuestion(CallbackQuery query)
        {
            int index = Global.GetQuestionIndex(query.Message.Text);
            if (index < 10)
            {
                currentQuestionByUsers[query.From] = questionsByUsers[query.From][index];
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: currentQuestionByUsers[query.From].Number + ". " + currentQuestionByUsers[query.From].Question, replyMarkup: KeyboardFunctions.GetAnswers(currentQuestionByUsers[query.From]));
            }
            else
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: $"You passed the quiz. Your score: {currentScoreByUsers[query.From]}");
        }

        private void ResetQuiz(CallbackQuery query)
        {
            var quest = Global.GetDataFromJson(query.Data.Replace(" ", string.Empty));
            if (!questionsByUsers.ContainsKey(query.From))
            {
                questionsByUsers.Add(query.From, quest);
                currentQuestionByUsers.Add(query.From, quest[0]);
                currentScoreByUsers.Add(query.From, 0);
            }
            else
            {
                questionsByUsers[query.From] = quest;
                currentQuestionByUsers[query.From] = quest[0];
                currentScoreByUsers[query.From] = 0;
            }
        }

        private bool CheckAnswer(CallbackQuery query)
        {
            if (currentQuestionByUsers[query.From] == null)
                return false;

            int rightAnswer = currentQuestionByUsers[query.From].Answer;
            string rightAnswerLetter = query.Data.Substring(0, 1);
            return (rightAnswer == 1 && rightAnswerLetter == "A") ||
                   (rightAnswer == 2 && rightAnswerLetter == "B") ||
                   (rightAnswer == 3 && rightAnswerLetter == "C") ||
                   (rightAnswer == 4 && rightAnswerLetter == "D");
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
