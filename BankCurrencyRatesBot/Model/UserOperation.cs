using System;
using System.Collections.Generic;

namespace BankCurrencyRatesBot.Model
{
    public class UserOperation
    {
        public UserOperationType? Type { get; set; }
        public ExchangeCurrencyOperation ExchangeCurrencyOperation { get; set; }
        public CurrencyRateOperation CurrencyRateOperation { get; set; }

        public enum UserOperationType
        {
            ExchangeCurrency,
            CurrencyRate
        }
    }

    public class ExchangeCurrencyOperation
    {
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Result { get; set; }

    }

    public class CurrencyRateOperation
    {
        public CurrencyType? Type { get; set; }
        public ICollection<CurrencyCode> CurrencyCodes { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }

        public enum CurrencyType
        {
            Today = 1,
            Yesterday = 2,
            OneWeek = 3,
            OneMonth = 4,
            ChoosePeriod = 5,
            Top5 = 6,
            AllCurrency = 7
        }
    }
}