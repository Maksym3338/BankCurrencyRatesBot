using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankCurrencyRatesBot.Enum;
using BankCurrencyRatesBot.Model;
using BankCurrencyRatesBot.NewModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using CurrencyCode = BankCurrencyRatesBot.Enum.CurrencyCode;


namespace BankCurrencyRatesBot
{
    public class TelegramHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private static TelegramBotClient Bot;
        private readonly NbuClient _nbuClient;
        private readonly IOptions<BotSettings> _botSettings;

        public static List<User> DbUser = new List<User>();


        public TelegramHostedService(ILogger<TelegramHostedService> logger, NbuClient nbuClient, IOptions<BotSettings> botSettings)
        {
            _logger = logger;
            _nbuClient = nbuClient;
            _botSettings = botSettings;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Telegram Background Service is starting.");

            Bot = new TelegramBotClient(_botSettings.Value.Token);

            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;

            Bot.StartReceiving(Array.Empty<UpdateType>(), cancellationToken);

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Telegram Background Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Telegram Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            Bot.StopReceiving();

            return Task.CompletedTask;
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text)
                return;

            try
            {
                var user = DbUser.FirstOrDefault(x => x.Id == message.Chat.Id);

                if (user == null)
                {
                    user = new User(messageEventArgs.Message.Chat.Id);
                    DbUser.Add(user);
                    await SendLanguageMessage(message.Chat.Id, message.Chat.FirstName, message.Text);
                    return;
                }

                var commandKey = ChatHelpers.GetKeyCommand(message.Text, user.LocalizedCommands);

                switch (commandKey)
                {
                    case KeyCommands.Start:
                        {
                            await SendLanguageMessage(message.Chat.Id, message.Chat.FirstName, message.Text);
                            return;
                        }
                    case KeyCommands.ComeBack:
                        {
                            var languageKeyboardMarkup = MyKeyboardMarkup.GetLanguageKeyboardMarkup(user.LocalizedCommands);
                            await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.StartMessage],
                                languageKeyboardMarkup);
                            return;
                        }
                    case KeyCommands.Ukrainian:
                    case KeyCommands.Russian:
                    case KeyCommands.English:
                        {
                            user.Language = LanguageHelper.GetLanguage(message.Chat.FirstName, message.Text);
                            user.SetLocalizedCommands(message.Chat.FirstName, message.Text);
                            var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands);
                            await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseAction],
                                replyKeyboardMarkup);
                            return;
                        }
                    case KeyCommands.CurrencyRate:
                        {
                            user.Operation.Type = UserOperationType.CurrencyRate;
                            break;
                        }
                    case KeyCommands.ExchangeCurrency:
                        {
                            user.Operation.Type = UserOperationType.ExchangeCurrency;
                            break;
                        }
                }


                switch (user.Operation.Type)
                {
                    case UserOperationType.ExchangeCurrency:
                        {
                            if (user.Operation.ExchangeCurrencyOperation == null)
                            {
                                user.Operation.ExchangeCurrencyOperation = new ExchangeCurrencyOperation();
                                await Bot.SendTextMessageAsync(message.Chat.Id, user.LocalizedCommands[KeyCommands.FirstExchangeCurrencyCodeMessage], replyMarkup: new ReplyKeyboardRemove());
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();
                                await SendMessage(message.Chat.Id, codeMessage);

                                return;
                            }

                            if (string.IsNullOrEmpty(user.Operation.ExchangeCurrencyOperation.CurrencyFrom))
                            {
                                var messageText = message.Text.Trim('/');
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var currenciesCodesList = rates.FirstOrDefault(x => x.Code == messageText);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();

                                if (currenciesCodesList != null)
                                {
                                    user.Operation.ExchangeCurrencyOperation.CurrencyFrom = messageText;
                                    await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.SecondExchangeCurrencyCodeMessage]);
                                    await SendMessage(message.Chat.Id, codeMessage);

                                    return;
                                }

                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.FirstExchangeCurrencyCodeMessage]);
                                await SendMessage(message.Chat.Id, codeMessage);

                                return;
                            }

                            if (string.IsNullOrEmpty(user.Operation.ExchangeCurrencyOperation.CurrencyTo))
                            {
                                var messageText = message.Text.Trim('/');
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var currenciesCodesList = rates.FirstOrDefault(x => x.Code == messageText);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();


                                if (currenciesCodesList != null)
                                {
                                    user.Operation.ExchangeCurrencyOperation.CurrencyTo = messageText;
                                    await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseExchangeAmountMessage]);

                                    return;
                                }


                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.SecondExchangeCurrencyCodeMessage]);
                                await SendMessage(message.Chat.Id, codeMessage);


                                return;
                            }

                            if (user.Operation.ExchangeCurrencyOperation.Amount.HasValue == false)
                            {
                                //TODO read about cultures
                                if (decimal.TryParse(message.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal result))
                                {
                                    user.Operation.ExchangeCurrencyOperation.Amount = result;
                                    await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseExchangeDayMessage]);

                                    return;
                                }

                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseExchangeAmountMessage]);

                                return;
                            }

                            if (user.Operation.ExchangeCurrencyOperation.Date.HasValue == false)
                            {
                                if (DateTime.TryParseExact(message.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                                {
                                    user.Operation.ExchangeCurrencyOperation.Date = result;
                                    var getCurrenciesRates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                    user.Operation.ExchangeCurrencyOperation.Result = ConversionHelper.ConvertFromOneAmountToAnother(
                                        user.Operation.ExchangeCurrencyOperation.CurrencyFrom,
                                        user.Operation.ExchangeCurrencyOperation.CurrencyTo,
                                        user.Operation.ExchangeCurrencyOperation.Amount,
                                        getCurrenciesRates);
                                    var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands);
                                    string exchangeResultMessage = " ";
                                    exchangeResultMessage +=
                                        $"{user.Operation.ExchangeCurrencyOperation.Amount}/{user.Operation.ExchangeCurrencyOperation.CurrencyFrom} =" +
                                        $" {user.Operation.ExchangeCurrencyOperation.Result}/{user.Operation.ExchangeCurrencyOperation.CurrencyTo}";
                                    await Bot.SendTextMessageAsync(message.Chat.Id, exchangeResultMessage, replyMarkup: replyKeyboardMarkup);

                                    user.Operation = new ChatOperation();

                                    return;
                                }

                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseExchangeDayMessage]);

                                return;
                            }

                            return;
                        }
                    case UserOperationType.CurrencyRate:
                        {
                            if (user.Operation.CurrencyRateOperation == null)
                            {
                                user.Operation.CurrencyRateOperation = new CurrencyRateOperation();
                                var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyKeyboardMarkup(user.LocalizedCommands);
                                await Bot.SendTextMessageAsync(message.Chat.Id,
                                    user.LocalizedCommands[KeyCommands.ChooseCurrency],
                                    replyMarkup: replyKeyboardMarkup);

                                return;
                            }

                            if (user.Operation.CurrencyRateOperation.Type == null)
                            {
                                if (System.Enum.TryParse(commandKey.ToString(), true, out CurrencyType currencyOperation))
                                {
                                    switch (currencyOperation)
                                    {
                                        case CurrencyType.Top5:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.Top5;
                                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                                var text = currencyRateMessages.GetRatesMessage(rates
                                                    .Where(x =>
                                                            x.Code == CurrencyCode.USD.ToString() ||
                                                            x.Code == CurrencyCode.EUR.ToString() ||
                                                            x.Code == CurrencyCode.GBP.ToString() ||
                                                            x.Code == CurrencyCode.CAD.ToString() ||
                                                            x.Code == CurrencyCode.RUB.ToString())
                                                        .OrderBy(x => x.Code).ToList());
                                                await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands));

                                                user.Operation = new ChatOperation();

                                                return;
                                            }
                                        case CurrencyType.AllCurrency:
                                            {
                                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.AllCurrency;
                                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                                var text = currencyRateMessages.GetRatesMessage(rates);
                                                await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands));

                                                user.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.Today:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.Today;
                                                await GetCurrencyRateResult(user, message.Chat.Id, DateTime.Today);

                                                user.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.Yesterday:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.Yesterday;
                                                await GetCurrencyRateResult(user, message.Chat.Id, DateTime.Today.AddDays(-1));

                                                user.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.OneWeek:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.OneWeek;
                                                await GetCurrencyRateResult(user, message.Chat.Id, DateTime.Today.AddDays(-7), DateTime.Today);

                                                user.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.OneMonth:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.OneMonth;
                                                await GetCurrencyRateResult(user, message.Chat.Id, DateTime.Today.AddMonths(-1), DateTime.Today);

                                                user.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.ChoosePeriod:
                                            {
                                                user.Operation.CurrencyRateOperation.Type = CurrencyType.ChoosePeriod;
                                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseFirstDay], new ReplyKeyboardRemove());

                                                return;
                                            }
                                        default:
                                            {
                                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgain]);
                                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseButtonBelow]);

                                                return;
                                            }
                                    }
                                }

                                if (System.Enum.TryParse(message.Text, true, out CurrencyCode currencyType))
                                {
                                    user.Operation.CurrencyRateOperation.CurrencyCodes = new List<NewModel.CurrencyCode>()
                                    {
                                        new NewModel.CurrencyCode()
                                        {
                                            Code = message.Text
                                        }
                                    };
                                    var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRatePeriodKeyboardMarkup(user.LocalizedCommands);
                                    await Bot.SendTextMessageAsync(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChoosePeriodMessage], replyMarkup: replyKeyboardMarkup);

                                    return;
                                }

                                return;
                            }

                            if (user.Operation.CurrencyRateOperation.StartDate == null ||
                                user.Operation.CurrencyRateOperation.EndDate == null)
                            {
                                if (DateTime.TryParseExact(message.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result))
                                {
                                    if (user.Operation.CurrencyRateOperation.StartDate == null)
                                    {
                                        user.Operation.CurrencyRateOperation.StartDate = result;
                                        await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.ChooseSecondDay]);

                                        return;
                                    }

                                    user.Operation.CurrencyRateOperation.EndDate = result;

                                    if (user.Operation.CurrencyRateOperation.EndDate == null)
                                    {
                                       throw new Exception();
                                    }

                                    var rates = await _nbuClient.GetCurrencyRatesListAsync(user.Operation.CurrencyRateOperation.StartDate.Value,user.Operation.CurrencyRateOperation.EndDate.Value);

                                    var currencyMessageGenerator = new CurrencyRateMessages(rates);
                                    var text = currencyMessageGenerator.GetCurrenciesWithRatesMessage(user);
                                    await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands));


                                    user.Operation = new ChatOperation();

                                    return;
                                }

                                await SendMessage(message.Chat.Id, user.LocalizedCommands[KeyCommands.TryAgainChosenPeriod]);

                                return;

                            }

                            return;
                        }

                    default:
                        {
                            await Bot.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Sorry, we don't understand your choice.\nTry again, please!Follow clear directions!!!",
                                replyMarkup: new ReplyKeyboardMarkup("/start"));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Sorry, something went wrong.\n Please try again.", replyMarkup: new ReplyKeyboardMarkup("/start"));

                _logger.LogError(ex, ex.Message);
            }
        }
        
        private static async Task SendLanguageMessage(long chatId, string userFirstName, string messageText)
        {
            var user = ChatHelpers.FindUser(chatId);
            user.SetLocalizedCommands(userFirstName, messageText);
            var languageKeyboardMarkup = MyKeyboardMarkup.GetLanguageKeyboardMarkup(user.LocalizedCommands);

            await Bot.SendTextMessageAsync(
                chatId: chatId,
                text: $"Hello {userFirstName}. Please choose LANGUAGE on Keyboard.\n" +
                      $"Добрий день {userFirstName}. Будь ласка оберіть мову.\n" +
                      $"Добрый день {userFirstName}. Пожалуйста выберите язык.",
                replyMarkup: languageKeyboardMarkup);
        }

        private async Task GetCurrencyRateResult(User user, long chatId, DateTime firstDay, DateTime? secondDay = null)
        {
            var rates = await _nbuClient.GetCurrencyRatesListAsync(firstDay, secondDay);
            var currencyRateMessages = new CurrencyRateMessages(rates);
            var text = currencyRateMessages.GetCurrenciesWithRatesMessage(user);
            await SendMessage(chatId, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(user.LocalizedCommands));
            user.Operation.CurrencyRateOperation.CurrencyCodes.Clear();
        }

        private static async Task SendMessage(long chatId, string text, IReplyMarkup replyKeyboardMarkup = null)
        {
            await Bot.SendTextMessageAsync(chatId, text, replyMarkup: replyKeyboardMarkup);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}