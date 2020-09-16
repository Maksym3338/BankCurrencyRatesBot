using System;
using BankCurrencyRatesBot.Attributes;
using Newtonsoft.Json;

namespace BankCurrencyRatesBot.Model
{
    public class Currency
    {
        [JsonProperty("r030")]
        public int Id { get; set; }
        [JsonProperty("txt")]
        public string Name { get; set; }
        [JsonProperty("rate")]
        public decimal Rate { get; set; }
        [JsonProperty("cc")]
        public string Code { get; set; }

        [JsonProperty("exchangedate")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ExchangeDate { get; set; }
    }
}