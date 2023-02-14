using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram_Bot.Models
{
    class QuestionModel
    {
        public int Number { get; set; }

        public string Question { get; set; }

        public string VariantA { get; set; }

        public string VariantB { get; set; }

        public string VariantC { get; set; }

        public string VariantD { get; set; }

        public int Answer { get; set; }
    }
}
