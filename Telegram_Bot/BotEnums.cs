using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Telegram_Bot
{
    internal class BotEnums
    {
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
    }
}
