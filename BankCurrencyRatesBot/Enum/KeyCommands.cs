namespace BankCurrencyRatesBot.Enum
{
    public enum KeyCommands
    {
        // Messages for User
        Start = 1,
        StartMessage,
        Sorry,
        ChooseButtonBelow,
        // Language
        English,
        Ukrainian,
        Russian,
        // Action
        CurrencyRate,
        ExchangeCurrency,
        ComeBack,
        ChooseAction,
        // Currency
        Usd,
        Eur,
        Uah,
        Gbp,
        Top5,
        AllCurrency,
        ChooseCurrency,
        // Period
        Today,
        Yesterday,
        OneWeek,
        OneMonth,
        ChoosePeriod,
        ChoosePeriodMessage,
        // Days
        ChosenDaysInPeriod,
        TryAgainChosenPeriod,
        ChooseFirstDay,
        ChooseSecondDay,
        // ExchangeCurrency

        ChosenAmount,
        FirstExchangeCurrencyCodeMessage,
        SecondExchangeCurrencyCodeMessage,

        ChooseExchangeAmountMessage,
        ChooseExchangeDayMessage,
        // ChoosePeriod
        TryAgain
    }
}