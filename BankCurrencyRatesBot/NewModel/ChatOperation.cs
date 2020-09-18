using BankCurrencyRatesBot.Model;

namespace BankCurrencyRatesBot.NewModel
{
    public class ChatOperation
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public UserOperation.UserOperationType? Type { get; set; }
        public int ExchangeCurrencyOperationId { get; set; }
        public int CurrencyRateOperationId { get; set; }
    }
}