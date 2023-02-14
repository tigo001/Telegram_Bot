using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Requests;
using Telegram_Bot.Models;
using Newtonsoft.Json;
using System.IO;

namespace Telegram_Bot
{
    class Global
    {
        public static List<QuestionModel> GetDataFromJson(string fileName)
        {
            string filePath = GetFilePath(fileName);
            string json = File.ReadAllText(filePath);
            QuestionModel[] questions = JsonConvert.DeserializeObject<QuestionModel[]>(json);

            return questions.ToList();
        }

        private static string GetFilePath(string fileName)
        {
            var res = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            res += $"\\Tests\\{fileName}.json";

            return res;
        }
    }
}
