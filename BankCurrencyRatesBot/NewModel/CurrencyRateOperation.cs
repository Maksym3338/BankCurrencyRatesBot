using System;
using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;

namespace BankCurrencyRatesBot.NewModel
{
    public class CurrencyRateOperation
    {
        public int Id { get; set; }
        public int ChatOperationId { get; set; }
        public CurrencyType? Type { get; set; }
        public int CurrencyCodesId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<CurrencyCode> CurrencyCodes { get; set; }

    }
}