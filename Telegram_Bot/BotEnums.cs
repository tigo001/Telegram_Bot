using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Telegram_Bot
{
    internal class BotEnums
    {
        #region Enums
        public enum Topics
        {
            HistoryOfArmenia = 0,
            Math = 1,
            Physics = 2
        }

        public enum Answers
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4
        }
        #endregion

        public static string[] GetEnumValues(Type enumType)
        {
            var values = Enum.GetNames(enumType);
            List<string> result = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var value = Regex.Split(values[i], @"(?<!^)(?=[A-Z])");
                var res = String.Join(" ", value);
                result.Add(res);
            }

            return result.ToArray();
        }

        public static Answers GetAnswers(string answerLetter)
        {
            switch(answerLetter)
            {
                case "A":
                    return Answers.A;
                case "B":
                    return Answers.B;
                case "C":
                    return Answers.C;
                case"D":
                    return Answers.D;
                default:
                    return Answers.A;
            }
        }
    }
}
