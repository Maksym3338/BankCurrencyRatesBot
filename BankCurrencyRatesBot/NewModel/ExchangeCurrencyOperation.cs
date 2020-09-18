using System;

namespace BankCurrencyRatesBot.NewModel
{
    public class ExchangeCurrencyOperation
    {
        public int Id { get; set; }
        public int ChatOperationId { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Result { get; set; }
    }
}