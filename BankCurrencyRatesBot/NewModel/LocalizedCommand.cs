using BankCurrencyRatesBot.Enum;

namespace BankCurrencyRatesBot.NewModel
{
    public class LocalizedCommand
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public KeyCommands KeyCommand { get; set; }
        public string Description { get; set; }
    }
}