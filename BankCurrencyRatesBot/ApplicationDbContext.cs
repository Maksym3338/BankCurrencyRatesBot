using BankCurrencyRatesBot.NewModel;
using Microsoft.EntityFrameworkCore;

namespace BankCurrencyRatesBot
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-IA30TLH;Database=BankCurrencyRateBot;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatOperation> ChatOperations { get; set; }
        public DbSet<LocalizedCommand> LocalizedCommands { get; set; }
        public DbSet<ExchangeCurrencyOperation> ExchangeCurrencyOperations { get; set; }
        public DbSet<CurrencyRateOperation> CurrencyRateOperations { get; set; }
        public DbSet<CurrencyCode> CurrencyCodes { get; set; }
    }
}