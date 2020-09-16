using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;
using Telegram.Bot.Types.ReplyMarkups;

namespace BankCurrencyRatesBot.Model
{
    public class MyKeyboardMarkup
    {
        // Get language KeyboardMarkup
        public static ReplyKeyboardMarkup GetLanguageKeyboardMarkup(Dictionary<KeyCommands, string> userLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] {userLocalizedCommands[KeyCommands.English]},
                    new KeyboardButton[] {userLocalizedCommands[KeyCommands.Russian]},
                    new KeyboardButton[] {userLocalizedCommands[KeyCommands.Ukrainian]},
                },
                true);
        }

        // Get KeyboardMarkup when we choose Currency Rate or Convert Currencies
        public static ReplyKeyboardMarkup GetCurrencyRateKeyboardMarkup(Dictionary<KeyCommands, string> userLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.CurrencyRate] },
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.ExchangeCurrency] },
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.ComeBack] }
                },
                true
            );
        }

        // Get exchange currencies codes KeyboardMarkup
        public static ReplyKeyboardMarkup GetCurrencyKeyboardMarkup(Dictionary<KeyCommands, string> userLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] {userLocalizedCommands[KeyCommands.Usd], userLocalizedCommands[KeyCommands.Eur], userLocalizedCommands[KeyCommands.Gbp]},
                    new KeyboardButton[] {userLocalizedCommands[KeyCommands.Top5]},
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.AllCurrency]},
                },
                true);
        }


        // Get exchange currencies codes KeyboardMarkup
        public static ReplyKeyboardMarkup GetCurrencyRatePeriodKeyboardMarkup(Dictionary<KeyCommands, string> userLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.Today], userLocalizedCommands[KeyCommands.Yesterday] },
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.OneWeek], userLocalizedCommands[KeyCommands.OneMonth] },
                    new KeyboardButton[] { userLocalizedCommands[KeyCommands.ChoosePeriod] },
                },
                true);
        }
    }
}