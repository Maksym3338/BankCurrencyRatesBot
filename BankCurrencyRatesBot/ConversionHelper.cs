using System.Collections.Generic;
using System.Linq;
using BankCurrencyRatesBot.Enum;
using BankCurrencyRatesBot.Model;

namespace BankCurrencyRatesBot
{
    public class ConversionHelper
    {
        public static void ConvertFromOneAmountToAnother(User user, List<Currency> getCurrenciesRates) // TODO rework with 3 parameters CurrencyFrom, CurrencyTo, Amount without user and List<Currency>
        {
            var getFirstCurrencyRate = getCurrenciesRates.FirstOrDefault(x => x.Code == user.Operation.ExchangeCurrencyOperation.CurrencyFrom);
            var getSecondCurrencyRate = getCurrenciesRates.FirstOrDefault(x => x.Code == user.Operation.ExchangeCurrencyOperation.CurrencyTo);


            if (user.Operation.ExchangeCurrencyOperation.CurrencyFrom == CurrencyType.UAH.ToString())
            {
                user.Operation.ExchangeCurrencyOperation.Result = user.Operation.ExchangeCurrencyOperation.Amount / getSecondCurrencyRate.Rate;
            }
            else if(user.Operation.ExchangeCurrencyOperation.CurrencyTo == CurrencyType.UAH.ToString())
            {
                user.Operation.ExchangeCurrencyOperation.Result = user.Operation.ExchangeCurrencyOperation.Amount * getFirstCurrencyRate.Rate;
            }
            else
            {
                user.Operation.ExchangeCurrencyOperation.Result = user.Operation.ExchangeCurrencyOperation.Amount * getFirstCurrencyRate.Rate / getSecondCurrencyRate.Rate;
            }
        }
    }
}