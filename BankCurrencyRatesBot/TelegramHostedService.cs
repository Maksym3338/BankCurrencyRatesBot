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

        public static List<Chat> DbChat = new List<Chat>();


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
                var chat = DbChat.FirstOrDefault(x => x.ChatId == message.Chat.Id);

                if (chat == null)
                {
                    chat = new Chat(messageEventArgs.Message.Chat.Id);
                    DbChat.Add(chat);
                    await SendLanguageMessage(message.Chat.Id, message.Chat.FirstName, message.Text);
                    return;
                }

                var commandKey = ChatHelpers.GetKeyCommand(message.Text, chat.LocalizedCommands);

                switch (commandKey)
                {
                    case KeyCommands.Start:
                        {
                            await SendLanguageMessage(message.Chat.Id, message.Chat.FirstName, message.Text);
                            return;
                        }
                    case KeyCommands.ComeBack:
                        {
                            var languageKeyboardMarkup = MyKeyboardMarkup.GetLanguageKeyboardMarkup(chat.LocalizedCommands);
                            await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.StartMessage],
                                languageKeyboardMarkup);
                            return;
                        }
                    case KeyCommands.Ukrainian:
                    case KeyCommands.Russian:
                    case KeyCommands.English:
                        {
                            chat.Language = LanguageHelper.GetLanguage(message.Chat.FirstName, message.Text);
                            chat.SetLocalizedCommands(message.Chat.FirstName, message.Text);
                            var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands);
                            await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseAction],
                                replyKeyboardMarkup);
                            return;
                        }
                    case KeyCommands.CurrencyRate:
                        {
                            chat.Operation.Type = ChatOperationType.CurrencyRate;
                            break;
                        }
                    case KeyCommands.ExchangeCurrency:
                        {
                            chat.Operation.Type = ChatOperationType.ExchangeCurrency;
                            break;
                        }
                }


                switch (chat.Operation.Type)
                {
                    case ChatOperationType.ExchangeCurrency:
                        {
                            if (chat.Operation.ExchangeCurrencyOperation == null)
                            {
                                chat.Operation.ExchangeCurrencyOperation = new ExchangeCurrencyOperation();
                                await Bot.SendTextMessageAsync(message.Chat.Id, chat.LocalizedCommands[KeyCommands.FirstExchangeCurrencyCodeMessage], replyMarkup: new ReplyKeyboardRemove());
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();
                                await SendMessage(message.Chat.Id, codeMessage);

                                return;
                            }

                            if (string.IsNullOrEmpty(chat.Operation.ExchangeCurrencyOperation.CurrencyFrom))
                            {
                                var messageText = message.Text.Trim('/');
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var currenciesCodesList = rates.FirstOrDefault(x => x.Code == messageText);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();

                                if (currenciesCodesList != null)
                                {
                                    chat.Operation.ExchangeCurrencyOperation.CurrencyFrom = messageText;
                                    await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.SecondExchangeCurrencyCodeMessage]);
                                    await SendMessage(message.Chat.Id, codeMessage);

                                    return;
                                }

                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.FirstExchangeCurrencyCodeMessage]);
                                await SendMessage(message.Chat.Id, codeMessage);

                                return;
                            }

                            if (string.IsNullOrEmpty(chat.Operation.ExchangeCurrencyOperation.CurrencyTo))
                            {
                                var messageText = message.Text.Trim('/');
                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                var currenciesCodesList = rates.FirstOrDefault(x => x.Code == messageText);
                                var codeMessage = currencyRateMessages.GetMessageRatesCodes();


                                if (currenciesCodesList != null)
                                {
                                    chat.Operation.ExchangeCurrencyOperation.CurrencyTo = messageText;
                                    await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseExchangeAmountMessage]);

                                    return;
                                }


                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.SecondExchangeCurrencyCodeMessage]);
                                await SendMessage(message.Chat.Id, codeMessage);


                                return;
                            }

                            if (chat.Operation.ExchangeCurrencyOperation.Amount.HasValue == false)
                            {
                                //TODO read about cultures
                                if (decimal.TryParse(message.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal result))
                                {
                                    chat.Operation.ExchangeCurrencyOperation.Amount = result;
                                    await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseExchangeDayMessage]);

                                    return;
                                }

                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseExchangeAmountMessage]);

                                return;
                            }

                            if (chat.Operation.ExchangeCurrencyOperation.Date.HasValue == false)
                            {
                                if (DateTime.TryParseExact(message.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                                {
                                    chat.Operation.ExchangeCurrencyOperation.Date = result;
                                    var getCurrenciesRates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                    chat.Operation.ExchangeCurrencyOperation.Result = ConversionHelper.ConvertFromOneAmountToAnother(
                                        chat.Operation.ExchangeCurrencyOperation.CurrencyFrom,
                                        chat.Operation.ExchangeCurrencyOperation.CurrencyTo,
                                        chat.Operation.ExchangeCurrencyOperation.Amount,
                                        getCurrenciesRates);
                                    var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands);
                                    string exchangeResultMessage = " ";
                                    exchangeResultMessage +=
                                        $"{chat.Operation.ExchangeCurrencyOperation.Amount}/{chat.Operation.ExchangeCurrencyOperation.CurrencyFrom} =" +
                                        $" {chat.Operation.ExchangeCurrencyOperation.Result}/{chat.Operation.ExchangeCurrencyOperation.CurrencyTo}";
                                    await Bot.SendTextMessageAsync(message.Chat.Id, exchangeResultMessage, replyMarkup: replyKeyboardMarkup);

                                    chat.Operation = new ChatOperation();

                                    return;
                                }

                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgain]);
                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseExchangeDayMessage]);

                                return;
                            }

                            return;
                        }
                    case ChatOperationType.CurrencyRate:
                        {
                            if (chat.Operation.CurrencyRateOperation == null)
                            {
                                chat.Operation.CurrencyRateOperation = new CurrencyRateOperation();
                                var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyKeyboardMarkup(chat.LocalizedCommands);
                                await Bot.SendTextMessageAsync(message.Chat.Id,
                                    chat.LocalizedCommands[KeyCommands.ChooseCurrency],
                                    replyMarkup: replyKeyboardMarkup);

                                return;
                            }

                            if (chat.Operation.CurrencyRateOperation.Type == null)
                            {
                                if (System.Enum.TryParse(commandKey.ToString(), true, out CurrencyType currencyOperation))
                                {
                                    switch (currencyOperation)
                                    {
                                        case CurrencyType.Top5:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.Top5;
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
                                                await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands));

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }
                                        case CurrencyType.AllCurrency:
                                            {
                                                var rates = await _nbuClient.GetCurrencyRatesListAsync(DateTime.Today);
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.AllCurrency;
                                                var currencyRateMessages = new CurrencyRateMessages(rates);
                                                var text = currencyRateMessages.GetRatesMessage(rates);
                                                await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands));

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.Today:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.Today;
                                                await GetCurrencyRateResult(chat, message.Chat.Id, DateTime.Today);

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.Yesterday:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.Yesterday;
                                                await GetCurrencyRateResult(chat, message.Chat.Id, DateTime.Today.AddDays(-1));

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.OneWeek:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.OneWeek;
                                                await GetCurrencyRateResult(chat, message.Chat.Id, DateTime.Today.AddDays(-7), DateTime.Today);

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.OneMonth:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.OneMonth;
                                                await GetCurrencyRateResult(chat, message.Chat.Id, DateTime.Today.AddMonths(-1), DateTime.Today);

                                                chat.Operation = new ChatOperation();

                                                return;
                                            }

                                        case CurrencyType.ChoosePeriod:
                                            {
                                                chat.Operation.CurrencyRateOperation.Type = CurrencyType.ChoosePeriod;
                                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseFirstDay], new ReplyKeyboardRemove());

                                                return;
                                            }
                                        default:
                                            {
                                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgain]);
                                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseButtonBelow]);

                                                return;
                                            }
                                    }
                                }

                                if (System.Enum.TryParse(message.Text, true, out CurrencyCode currencyType))
                                {
                                    chat.Operation.CurrencyRateOperation.CurrencyCodes = new List<NewModel.CurrencyCode>()
                                    {
                                        new NewModel.CurrencyCode()
                                        {
                                            Code = message.Text
                                        }
                                    };
                                    var replyKeyboardMarkup = MyKeyboardMarkup.GetCurrencyRatePeriodKeyboardMarkup(chat.LocalizedCommands);
                                    await Bot.SendTextMessageAsync(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChoosePeriodMessage], replyMarkup: replyKeyboardMarkup);

                                    return;
                                }

                                return;
                            }

                            if (chat.Operation.CurrencyRateOperation.StartDate == null ||
                                chat.Operation.CurrencyRateOperation.EndDate == null)
                            {
                                if (DateTime.TryParseExact(message.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result))
                                {
                                    if (chat.Operation.CurrencyRateOperation.StartDate == null)
                                    {
                                        chat.Operation.CurrencyRateOperation.StartDate = result;
                                        await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.ChooseSecondDay]);

                                        return;
                                    }

                                    chat.Operation.CurrencyRateOperation.EndDate = result;

                                    if (chat.Operation.CurrencyRateOperation.EndDate == null)
                                    {
                                       throw new Exception();
                                    }

                                    var rates = await _nbuClient.GetCurrencyRatesListAsync(chat.Operation.CurrencyRateOperation.StartDate.Value,chat.Operation.CurrencyRateOperation.EndDate.Value);

                                    var currencyMessageGenerator = new CurrencyRateMessages(rates);
                                    var text = currencyMessageGenerator.GetCurrenciesWithRatesMessage(chat);
                                    await SendMessage(message.Chat.Id, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands));


                                    chat.Operation = new ChatOperation();

                                    return;
                                }

                                await SendMessage(message.Chat.Id, chat.LocalizedCommands[KeyCommands.TryAgainChosenPeriod]);

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
            var chat = ChatHelpers.FindChat(chatId);
            chat.SetLocalizedCommands(userFirstName, messageText);
            var languageKeyboardMarkup = MyKeyboardMarkup.GetLanguageKeyboardMarkup(chat.LocalizedCommands);

            await Bot.SendTextMessageAsync(
                chatId: chatId,
                text: $"Hello {userFirstName}. Please choose LANGUAGE on Keyboard.\n" +
                      $"Добрий день {userFirstName}. Будь ласка оберіть мову.\n" +
                      $"Добрый день {userFirstName}. Пожалуйста выберите язык.",
                replyMarkup: languageKeyboardMarkup);
        }

        private async Task GetCurrencyRateResult(Chat chat, long chatId, DateTime firstDay, DateTime? secondDay = null)
        {
            var rates = await _nbuClient.GetCurrencyRatesListAsync(firstDay, secondDay);
            var currencyRateMessages = new CurrencyRateMessages(rates);
            var text = currencyRateMessages.GetCurrenciesWithRatesMessage(chat);
            await SendMessage(chatId, text, MyKeyboardMarkup.GetCurrencyRateKeyboardMarkup(chat.LocalizedCommands));
            chat.Operation.CurrencyRateOperation.CurrencyCodes.Clear();
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