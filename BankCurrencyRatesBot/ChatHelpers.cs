using System.Collections.Generic;
using System.Linq;
using User = BankCurrencyRatesBot.Model.User;

namespace BankCurrencyRatesBot
{
    public class ChatHelpers
    {
        // Find User
        public static User FindUser(long userID)
        {
            var dbUser = TelegramHostedService.DbUser; //TODO rework
            var user = dbUser.FirstOrDefault(x => x.Id == userID);
            return user;
        }

        // Get command key from dictionary
        public static T GetKeyCommand<T>(string messageText, Dictionary<T, string> dictionary) where T: System.Enum
        {
            var command = dictionary.FirstOrDefault(x => x.Value == messageText);
            return command.Key == null ? default : command.Key;
        }
    }
}