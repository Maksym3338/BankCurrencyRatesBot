using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace BankCurrencyRatesBot
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                    configApp.AddEnvironmentVariables();
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddCommandLine(args);
                    configApp.AddUserSecrets<Program>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TelegramHostedService>();
                    services.AddHttpClient<NbuClient>("nbuClient", c =>
                    {
                        c.BaseAddress = new Uri("https://bank.gov.ua/");
                    }).AddPolicyHandler(GetRetryPolicy());
                    services.Configure<BotSettings>(hostContext.Configuration);
                });

            await hostBuilder.RunConsoleAsync();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var httpStatusCodesWorthRetrying = new List<HttpStatusCode> {
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout // 504
            };

            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}