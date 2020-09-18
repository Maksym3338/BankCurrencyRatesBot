using System;
using System.Collections.Generic;
using System.Linq;
using BankCurrencyRatesBot.Enum;
using BankCurrencyRatesBot.Model;

namespace BankCurrencyRatesBot
{
    public class ConversionHelper
    {
        public static decimal? ConvertFromOneAmountToAnother(string currencyFrom, string currencyTo, decimal? amount, List<Currency> currenciesList)
        {
            var fromCurrency = currenciesList.FirstOrDefault(x => x.Code == currencyFrom);
            var toCurrency = currenciesList.FirstOrDefault(x => x.Code == currencyTo);

            if (fromCurrency == null || toCurrency == null)
            {
                throw new NullReferenceException();
            }


            if (fromCurrency.Code == CurrencyType.UAH.ToString())
            {
                return amount / toCurrency.Rate;
            }

            if (toCurrency.Code == CurrencyType.UAH.ToString())
            {
                return amount * fromCurrency.Rate;
            }

            return amount * fromCurrency.Rate / toCurrency.Rate;
        }
    }
}