using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;
using Telegram.Bot.Types.ReplyMarkups;

namespace BankCurrencyRatesBot.Model
{
    public class MyKeyboardMarkup
    {
        // Get language KeyboardMarkup
        public static ReplyKeyboardMarkup GetLanguageKeyboardMarkup(Dictionary<KeyCommands, string> chatLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] {chatLocalizedCommands[KeyCommands.English]},
                    new KeyboardButton[] {chatLocalizedCommands[KeyCommands.Russian]},
                    new KeyboardButton[] {chatLocalizedCommands[KeyCommands.Ukrainian]},
                },
                true);
        }

        // Get KeyboardMarkup when we choose Currency Rate or Convert Currencies
        public static ReplyKeyboardMarkup GetCurrencyRateKeyboardMarkup(Dictionary<KeyCommands, string> chatLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.CurrencyRate] },
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.ExchangeCurrency] },
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.ComeBack] }
                },
                true
            );
        }

        // Get exchange currencies codes KeyboardMarkup
        public static ReplyKeyboardMarkup GetCurrencyKeyboardMarkup(Dictionary<KeyCommands, string> chatLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] {chatLocalizedCommands[KeyCommands.Usd], chatLocalizedCommands[KeyCommands.Eur], chatLocalizedCommands[KeyCommands.Gbp]},
                    new KeyboardButton[] {chatLocalizedCommands[KeyCommands.Top5]},
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.AllCurrency]},
                },
                true);
        }


        // Get exchange currencies codes KeyboardMarkup
        public static ReplyKeyboardMarkup GetCurrencyRatePeriodKeyboardMarkup(Dictionary<KeyCommands, string> chatLocalizedCommands)
        {
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.Today], chatLocalizedCommands[KeyCommands.Yesterday] },
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.OneWeek], chatLocalizedCommands[KeyCommands.OneMonth] },
                    new KeyboardButton[] { chatLocalizedCommands[KeyCommands.ChoosePeriod] },
                },
                true);
        }
    }
}