using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;

namespace BankCurrencyRatesBot.NewModel
{
    public class Chat
    {
        public long ChatId { get; set; }
        public Languages Language { get; set; }
        public int OperationId { get; set; }
        public int LocalizedCommandId { get; set; }

        public Dictionary<KeyCommands, string> LocalizedCommands { get; private set; }
        public ChatOperation Operation { get; set; }

        public Chat(long id)
        {
            ChatId = id;
            LocalizedCommands = new Dictionary<KeyCommands, string>();
            Operation = new ChatOperation();
        }

        public void SetLocalizedCommands(string userFirstName, string messageText)
        {
            LocalizedCommands = LanguageHelper.ChooseLanguage(userFirstName, messageText);
        }
    }
}