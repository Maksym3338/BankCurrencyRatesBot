using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;
using Languages = BankCurrencyRatesBot.Enum.Languages;

namespace BankCurrencyRatesBot.Model
{
    public class User
    {
        public long Id { get; }
        public Languages Language { get; set; } 
        public Dictionary<KeyCommands, string> LocalizedCommands { get; private set; }
        public UserOperation Operation { get; set; }

        public User(long id)
        {
            Id = id;
            LocalizedCommands = new Dictionary<KeyCommands, string>();
            Operation = new UserOperation();
        }

        public void SetLocalizedCommands(string userFirstName, string messageText)
        {
            LocalizedCommands = LanguageHelper.ChooseLanguage(userFirstName, messageText);
        }
    }
}