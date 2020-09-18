using BankCurrencyRatesBot.Enum;

namespace BankCurrencyRatesBot.NewModel
{
    public class Chat
    {
        public long ChatId { get; set; }
        public Languages Language { get; set; }
        public int OperationId { get; set; }
        public int LocalizedCommandId { get; set; }
    }
}