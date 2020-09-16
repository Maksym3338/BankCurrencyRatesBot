using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BankCurrencyRatesBot.Model;
using Newtonsoft.Json;

namespace BankCurrencyRatesBot
{
    public class NbuClient
    {
        private readonly HttpClient _httpClient;

        public NbuClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<Currency>> GetCurrencyRatesListAsync(DateTime firstDate, DateTime? lastDate = null)
        {
            var bankCurrencyRates = new List<Currency>();
            if (lastDate == null)
            {
                lastDate = firstDate;
            }
            var extendedDate = firstDate;
            while (extendedDate <= lastDate)
            {
                var uri = $"NBUStatService/v1/statdirectory/exchange?date={extendedDate:yyyyMMdd}&json";
                var response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode == false)
                {
                    throw new HttpRequestException($"Failed with status code - {response.StatusCode}.");
                }

                var resultString = await response.Content.ReadAsStringAsync();

                var currencyRates = JsonConvert.DeserializeObject<List<Currency>>(resultString);

                bankCurrencyRates.AddRange(currencyRates);
                bankCurrencyRates.Add(new Currency
                {
                    Code = "UAH",
                    ExchangeDate = extendedDate,
                    Id = 980,
                    Name = "Українська гривня",
                    Rate = 1
                });

                extendedDate = extendedDate.AddDays(1);
            }

            return bankCurrencyRates;
        }
    }
}