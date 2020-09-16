using Newtonsoft.Json.Converters;

namespace BankCurrencyRatesBot.Attributes
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd.MM.yyyy";
        }
    }
}