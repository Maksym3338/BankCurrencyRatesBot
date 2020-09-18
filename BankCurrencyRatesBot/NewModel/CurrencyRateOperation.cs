using System;

namespace BankCurrencyRatesBot.NewModel
{
    public class CurrencyRateOperation
    {
        public int Id { get; set; }
        public int ChatOperationId { get; set; }
        public Model.CurrencyRateOperation.CurrencyType? Type { get; set; }
        public int CurrencyCodesId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}