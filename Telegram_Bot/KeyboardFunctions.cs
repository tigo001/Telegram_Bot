using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_Bot.Models;

namespace Telegram_Bot
{
    class KeyboardFunctions
    {
        public static InlineKeyboardMarkup GetFastKeysOnStart()
        {
            var fastKeys = new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Choose test"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Help"),
                }
            };
            return new InlineKeyboardMarkup(fastKeys);
        }

        public static InlineKeyboardMarkup GetTopics()
        {
            var topicsKeysArray = BotEnums.GetEnumValues(typeof(BotEnums.Topics));
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[topicsKeysArray.Length][];
            for (int i = 0; i < topicsKeysArray.Length; i++)
            {
                keyboard[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(topicsKeysArray[i].ToString()) };
            }

            return keyboard;
        }

        public static InlineKeyboardMarkup GetAnswers(QuestionModel quest)
        {
            var answersKeysArray = BotEnums.GetEnumValues(typeof(BotEnums.Answers));
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[answersKeysArray.Length][];
            for (int i = 0; i < answersKeysArray.Length; i++)
            {
                keyboard[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(answersKeysArray[i].ToString() + ". " + GetVariantText(i + 1, quest)) };
            }

            return keyboard;
        }

        private static string GetVariantText(int index, QuestionModel quest)
        {
            switch (index)
            {
                case 1:
                    return quest.VariantA;
                case 2:
                    return quest.VariantB;
                case 3:
                    return quest.VariantC;
                case 4:
                    return quest.VariantD;
                default:
                    return "";
            }
        }
    }
}
