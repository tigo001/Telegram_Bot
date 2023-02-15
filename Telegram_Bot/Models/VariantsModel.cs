using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram_Bot.Models
{
    class VariantsModel
    {
        public BotEnums.Answers Variant { get; set; }

        public string Text { get; set; }

        public VariantsModel (string variant)
        {
            Variant = GetVariantEnum(variant.Substring(0, 1));
            Text = variant.Substring(3);
        }

        private BotEnums.Answers GetVariantEnum(string variantLetter)
        {
            switch(variantLetter)
            {
                case "A":
                    return BotEnums.Answers.A;
                case "B":
                    return BotEnums.Answers.B;
                case "C":
                    return BotEnums.Answers.C;
                case "D":
                    return BotEnums.Answers.D;
                default:
                    //this case can't be reached, but without default we can't use switch function
                    return BotEnums.Answers.A;
            }
        }
    }
}
