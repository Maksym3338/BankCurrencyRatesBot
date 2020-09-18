using System;
using System.Collections.Generic;
using System.Linq;
using BankCurrencyRatesBot.Model;
using User = BankCurrencyRatesBot.Model.User;

namespace BankCurrencyRatesBot
{
    public class CurrencyRateMessages
    {
        private readonly IReadOnlyCollection<Currency> _allCurrencies;

        public CurrencyRateMessages(IReadOnlyCollection<Currency> allCurrencies)
        {
            _allCurrencies = allCurrencies;
        }

        public string GetCurrenciesWithRatesMessage(User user)
        {
            var listCodes = user.Operation.CurrencyRateOperation.CurrencyCodes.Select(x => x.Code);
            var getRates = _allCurrencies.Where(x => listCodes.Contains(x.Code)).ToList();
            var text = GetRatesMessage(getRates);
            return text;
        }

        public string GetRatesMessage(List<Currency> currenciesList)
        {
            var rates = currenciesList.OrderBy(x => x.Code).ToList();

            var ratesMessage = "";

            foreach (var rate in rates)
            {
                ratesMessage += $"{rate.Code} - {rate.Rate} - {rate.ExchangeDate:dd.MM.yyyy}{Environment.NewLine}";
            }

            return ratesMessage;
        }

        public string GetMessageRatesCodes()
        {
            var codeMessage = "";

            foreach (var rate in _allCurrencies)
            {
                codeMessage += $"/{rate.Code}{Environment.NewLine}";
            }


            return codeMessage;
        }
    }
}