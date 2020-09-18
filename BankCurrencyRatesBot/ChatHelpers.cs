using System.Collections.Generic;
using System.Linq;
using BankCurrencyRatesBot.NewModel;

namespace BankCurrencyRatesBot
{
    public class ChatHelpers
    {
        // Find Chat
        public static Chat FindChat(long chatId)
        {
            var chatDb = TelegramHostedService.DbChat; //TODO rework
            var chat = chatDb.FirstOrDefault(x => x.ChatId == chatId);
            return chat;
        }

        // Get command key from dictionary
        public static T GetKeyCommand<T>(string messageText, Dictionary<T, string> dictionary) where T: System.Enum
        {
            var command = dictionary.FirstOrDefault(x => x.Value == messageText);
            return command.Key == null ? default : command.Key;
        }
    }
}