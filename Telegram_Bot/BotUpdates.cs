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
        static Dictionary<long, List<QuestionModel>> questionsByUsers;
        static Dictionary<long, QuestionModel> currentQuestionByUsers;
        static Dictionary<long, int> currentScoreByUsers;
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
            var quest = currentQuestionByUsers[query.From.Id];
            long chatId = query.Message.Chat.Id;

            switch (query.Data)
            {
                case "Choose test":
                    await BotClient.SendTextMessageAsync(chatId: chatId, text: "Choose topic.", replyMarkup: KeyboardFunctions.GetTopics());
                    break;

                case "History Of Armenia":
                case "Math":
                case "Physics":
                    await BotClient.SendTextMessageAsync(chatId: chatId, text: "Ok, let's start.");
                    ResetQuiz(query);
                    await BotClient.SendTextMessageAsync(chatId: chatId, text: quest.Number + ". " + quest.Question, replyMarkup: KeyboardFunctions.GetAnswers(quest));
                    break;

                default:
                    if(CheckAnswer(query))
                    {
                        await BotClient.SendTextMessageAsync(chatId: chatId, text: "You're right.");
                        currentScoreByUsers[query.From.Id]++;
                    }
                    else
                        await BotClient.SendTextMessageAsync(chatId: chatId, text: "Right answer is " + GetAnswer(quest.Answer));
                    GiveNextQuestion(query);
                    break;
            }
        }

        private async void GiveNextQuestion(CallbackQuery query)
        {
            int index = Global.GetQuestionIndex(query.Message.Text);
            long userId = query.From.Id;
            if (index < 10)
            {
                currentQuestionByUsers[userId] = questionsByUsers[userId][index];
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: currentQuestionByUsers[userId].Number + ". " + currentQuestionByUsers[userId].Question, replyMarkup: KeyboardFunctions.GetAnswers(currentQuestionByUsers[userId]));
            }
            else
            {
                await BotClient.SendTextMessageAsync(chatId: query.Message.Chat.Id, text: $"You passed the quiz. Your score: {currentScoreByUsers[userId]}");
                questionsByUsers.Remove(userId);
                currentQuestionByUsers.Remove(userId);
                currentScoreByUsers.Remove(userId);
            }
        }

        private void ResetQuiz(CallbackQuery query)
        {
            var quest = Global.GetDataFromJson(query.Data.Replace(" ", string.Empty));
            if (questionsByUsers == null)
                questionsByUsers = new Dictionary<long, List<QuestionModel>>();
            if (currentQuestionByUsers == null)
                currentQuestionByUsers = new Dictionary<long, QuestionModel>();
            if (currentScoreByUsers == null)
                currentScoreByUsers = new Dictionary<long, int>();
            long userId = query.From.Id;

            if (!questionsByUsers.ContainsKey(userId))
            {
                questionsByUsers.Add(userId, quest);
                currentQuestionByUsers.Add(userId, quest[0]);
                currentScoreByUsers.Add(userId, 0);
            }
            else
            {
                questionsByUsers[userId] = quest;
                currentQuestionByUsers[userId] = quest[0];
                currentScoreByUsers[userId] = 0;
            }
        }

        private bool CheckAnswer(CallbackQuery query)
        {
            if (currentQuestionByUsers[query.From.Id] == null)
                return false;

            int rightAnswer = currentQuestionByUsers[query.From.Id].Answer;
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
