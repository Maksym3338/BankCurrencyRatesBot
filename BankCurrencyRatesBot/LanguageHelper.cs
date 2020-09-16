using System.Collections.Generic;
using BankCurrencyRatesBot.Enum;

namespace BankCurrencyRatesBot
{
    public class LanguageHelper
    {
        public static Languages GetLanguage(string userFirstName, string messageText)
        {
            var localizedCommandsList = CreateLocalizedCommandList(userFirstName);

            foreach (var localizedCommands in localizedCommandsList)
            {
                var language = ChatHelpers.GetKeyCommand(messageText, localizedCommands);

                switch (language)
                {
                    case KeyCommands.Ukrainian:
                        return Languages.Ukrainian;
                    case KeyCommands.Russian:
                        return Languages.Russian;
                    case KeyCommands.English:
                        return Languages.English;
                }
            }

            return Languages.English;
        }

        public static Dictionary<KeyCommands, string> ChooseLanguage(string userFirstName, string messageText)
        {
            var localizedCommandsList = CreateLocalizedCommandList(userFirstName);

            foreach (var localizedCommands in localizedCommandsList)
            {
                var language = ChatHelpers.GetKeyCommand(messageText, localizedCommands);

                switch (language)
                {
                    case KeyCommands.Ukrainian:
                        return Ukrainian(userFirstName);
                    case KeyCommands.Russian:
                        return Russian(userFirstName);
                    case KeyCommands.English:
                        return English(userFirstName);
                }
            }

            return English(userFirstName);
        }

        public static List<Dictionary<KeyCommands, string>> CreateLocalizedCommandList(string userFirstName)
        {
            var localizedCommandsList = new List<Dictionary<KeyCommands, string>>
            {
                English(userFirstName), Russian(userFirstName), Ukrainian(userFirstName)
            };

            return localizedCommandsList;
        }


        private static Dictionary<KeyCommands, string> English(string userFirstName)
        {
            return new Dictionary<KeyCommands, string>
            {
                // Messages for User
                {KeyCommands.Start, "/start"},
                {KeyCommands.StartMessage, $"Hello {userFirstName}. Please choose LANGUAGE on Keyboard."},
                {KeyCommands.Sorry,"Sorry, we don't understand your choice.\nTry again, please!Follow clear directions!!!"},
                {KeyCommands.TryAgain, "Try again or press /start and start over!"},
                {KeyCommands.ChooseButtonBelow, "Choose button below!"},



                // Language
                {KeyCommands.English, "English"},
                {KeyCommands.Ukrainian, "Ukrainian"},
                {KeyCommands.Russian, "Russian"},

                // Action
                {KeyCommands.CurrencyRate, "Currency rate"},
                {KeyCommands.ExchangeCurrency, "Convert currencies"},
                {KeyCommands.ComeBack, "<= Come back"},
                {KeyCommands.ChooseAction, $"Hello {userFirstName}. Choose your action!"},

                // Currency
                {KeyCommands.Usd, "USD"},
                {KeyCommands.Eur, "EUR"},
                {KeyCommands.Gbp, "GBP"},
                {KeyCommands.Top5, "Top 5 currencies today"},
                {KeyCommands.AllCurrency, "All currency rate today"},
                {KeyCommands.ChooseCurrency, $"{userFirstName}, please choose CURRENCY on Keyboard!"},

                // Period
                {KeyCommands.Today, "Today"},
                {KeyCommands.Yesterday, "Yesterday"},
                {KeyCommands.OneWeek, "One week"},
                {KeyCommands.OneMonth, "One month"},
                {KeyCommands.ChoosePeriod, "Choose your period"},
                {KeyCommands.ChoosePeriodMessage, $"{userFirstName}, please choose PERIOD on Keyboard!"},

                // Days
                {KeyCommands.ChosenDaysInPeriod, "ChosenDaysInPeriod"},
                {KeyCommands.TryAgainChosenPeriod, "Please try again! Be attentive! \nExample - 01.01.2020. \nOr click on /start and start over"},
                {KeyCommands.ChooseFirstDay, "Choose your first date.\nExample - 01/01/2020."},
                {KeyCommands.ChooseSecondDay, "Choose your second date.\nExample - 01/01/2020."},

                // ExchangeCurrency
                {KeyCommands.ChosenAmount, "ChosenAmount"},
                {KeyCommands.FirstExchangeCurrencyCodeMessage, $"{userFirstName}, choose first currency code for exchange."},
                {KeyCommands.SecondExchangeCurrencyCodeMessage, $"{userFirstName}, choose second currency code for exchange."},
                {KeyCommands.ChooseExchangeAmountMessage, $"{userFirstName}, write the exchange amount. \nFor example - 500 or 500.00"},
                {KeyCommands.ChooseExchangeDayMessage, $"{userFirstName}, write day when you want make your exchange. \nFor example - 01/01/2020"},
            };
        }

        private static Dictionary<KeyCommands, string> Russian(string userFirstName)
        {
            return new Dictionary<KeyCommands, string>
            {
                // Messages for User
                {KeyCommands.Start, "/start"},
                {KeyCommands.StartMessage, $"Добрый день {userFirstName}. Пожалуйста выберите язык."},
                {KeyCommands.Sorry,"Извините, мы не понимаем ваш выбор.\nПожалуйста, попробуйте еще раз! Следуйте четким указаниям!!!"},
                {KeyCommands.TryAgain, "Попробуйте еще раз или нажмите /start и начните заново!"},
                {KeyCommands.ChooseButtonBelow, "Выберите кнопку ниже!"},



                // Language
                {KeyCommands.English, "Английский"},
                {KeyCommands.Ukrainian, "Украинский"},
                {KeyCommands.Russian, "Русский"},

                // Action
                {KeyCommands.CurrencyRate, "Курс валют"},
                {KeyCommands.ExchangeCurrency, "Обмен валют"},
                {KeyCommands.ComeBack, "<= Вернуться обратно"},
                {KeyCommands.ChooseAction, $"Добрый день {userFirstName}. Выберите действие!"},

                // Currency
                {KeyCommands.Usd, "USD"},
                {KeyCommands.Eur, "EUR"},
                {KeyCommands.Gbp, "GBP"},
                {KeyCommands.Top5, "Топ-5 валют сегодня"},
                {KeyCommands.AllCurrency, "Все курсы валют сегодня"},
                {KeyCommands.ChooseCurrency, $"{userFirstName}, пожалуйста, выберите ВАЛЮТУ на клавиатуре!"},

                // Period
                {KeyCommands.Today, "Сегодня"},
                {KeyCommands.Yesterday, "Вчера"},
                {KeyCommands.OneWeek, "Одна неделя"},
                {KeyCommands.OneMonth, "Один месяц"},
                {KeyCommands.ChoosePeriod, "Выберите свой период"},
                {KeyCommands.ChoosePeriodMessage, $"{userFirstName}, выберите промежуток времени!"},

                // Days
                {KeyCommands.ChosenDaysInPeriod, "ChosenDaysInPeriod"},
                {KeyCommands.TryAgainChosenPeriod, "Попробуйте еще раз! Будьте внимательны! \nПример - 01.01.2020. \nИли нажмите /start и начните заново"},
                {KeyCommands.ChooseFirstDay, "Выберите первый день периода.\nПример - 01/01/2020."},
                {KeyCommands.ChooseSecondDay, "Выберите последний день периода.\nПример - 01/01/2020."},

                // ExchangeCurrency
                {KeyCommands.ChosenAmount, "ChosenAmount"},
                {KeyCommands.FirstExchangeCurrencyCodeMessage, $"{userFirstName}, выберите первый код валюты для обмена."},
                {KeyCommands.SecondExchangeCurrencyCodeMessage, $"{userFirstName}, выберите второй код валюты для обмена."},
                {KeyCommands.ChooseExchangeAmountMessage, $"{userFirstName}, напишите сумму обмена. \nНапример - 500 или 500.00"},
                {KeyCommands.ChooseExchangeDayMessage, $"{userFirstName}, напишите день, когда вы хотите провести операцию обмена. \nНапример - 01/01/2020"},
            };
        }

        private static Dictionary<KeyCommands, string> Ukrainian(string userFirstName)
        {
            return new Dictionary<KeyCommands, string>
            {
                // Messages for User
                {KeyCommands.Start, "/start"},
                {KeyCommands.StartMessage, $"Добрий день {userFirstName}. Будь ласка оберіть мову."},
                {KeyCommands.Sorry,"На жаль, ми не розуміємо вашого вибору.\nСпробуйте ще раз, будь ласка! Дотримуйтесь чітких вказівок!!!"},
                {KeyCommands.TryAgain, "Спробуйте ще раз або натисніть /start і почніть спочатку!"},
                {KeyCommands.ChooseButtonBelow, "Оберіть кнопку нижче!"},



                // Language
                {KeyCommands.English, "Англійська"},
                {KeyCommands.Ukrainian, "Українська"},
                {KeyCommands.Russian, "Російська"},

                // Action
                {KeyCommands.CurrencyRate, "Курс валют"},
                {KeyCommands.ExchangeCurrency, "Обмін валют"},
                {KeyCommands.ComeBack, "<= Повернутися назад"},
                {KeyCommands.ChooseAction, $"Добрий день {userFirstName}. Оберіть дію!"},

                // Currency
                {KeyCommands.Usd, "USD"},
                {KeyCommands.Eur, "EUR"},
                {KeyCommands.Gbp, "GBP"},
                {KeyCommands.Top5, "Топ-5 валют сьогодні"},
                {KeyCommands.AllCurrency, "Усі курси валют сьогодні"},
                {KeyCommands.ChooseCurrency, $"{userFirstName}, будь ласка, оберіть ВАЛЮТУ на клавіатурі!"},

                // Period
                {KeyCommands.Today, "Сьогодні"},
                {KeyCommands.Yesterday, "Вчора"},
                {KeyCommands.OneWeek, "Один тиждень"},
                {KeyCommands.OneMonth, "Один місяць"},
                {KeyCommands.ChoosePeriod, "Оберіть свій проміжок часу"},
                {KeyCommands.ChoosePeriodMessage, $"{userFirstName}, оберіть проміжок часу!"},

                // Days
                {KeyCommands.ChosenDaysInPeriod, "ChosenDaysInPeriod"},
                {KeyCommands.TryAgainChosenPeriod, "Спробуйте ще раз! Будьте уважні!\nПриклад - 01/01/2020.\nАбо натисніть /start і почніть спочатку!"},
                {KeyCommands.ChooseFirstDay, "Оберіть перший день періоду.\nПриклад - 01/01/2020."},
                {KeyCommands.ChooseSecondDay, "Оберіть останій день періоду.\nПриклад - 01/01/2020."},

                // ExchangeCurrency
                {KeyCommands.ChosenAmount, "ChosenAmount"},
                {KeyCommands.FirstExchangeCurrencyCodeMessage, $"{userFirstName}, оберіть перший код валюти для обміну."},
                {KeyCommands.SecondExchangeCurrencyCodeMessage, $"{userFirstName}, оберіть другий код валюти для обміну."},
                {KeyCommands.ChooseExchangeAmountMessage, $"{userFirstName}, напишіть суму обміну. \nНаприклад - 500 або 500.00"},
                {KeyCommands.ChooseExchangeDayMessage, $"{userFirstName}, напишіть день, коли ви бажаєте провести операцію обміну валюти. \nНаприклад - 01/01/2020"},

            };
        }
    }
}