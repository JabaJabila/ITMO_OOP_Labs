using Banks.Accounts;
using Banks.BankSystem;
using Banks.Clients;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.EntityFrameworkStuff
{
    public sealed class BanksContext : DbContext
    {
        public BanksContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<CentralBank> CentralBanks { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CreditAccountConfig> CreditAccountConfigs { get; set; }
        public DbSet<DebitAccountConfig> DebitAccountConfigs { get; set; }
        public DbSet<DepositAccountConfig> DepositAccountConfigs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.Clients)
                .WithMany(c => c.Banks)
                .UsingEntity(e => e.ToTable("BanksToClientRelations"));
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.OnCreditAccountChangesSubscribers)
                .WithMany(c => c.CreditAccountChangesPublishers)
                .UsingEntity(e => e.ToTable("BanksToCreditAccountChangesSubscribers"));
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.OnDebitAccountChangesSubscribers)
                .WithMany(c => c.DebitAccountChangesPublishers)
                .UsingEntity(e => e.ToTable("BanksToDebitAccountChangesSubscribers"));
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.OnDepositAccountChangesSubscribers)
                .WithMany(c => c.DepositAccountChangesPublishers)
                .UsingEntity(e => e.ToTable("BanksToDepositAccountChangesSubscribers"));

            modelBuilder.Entity<DebitAccount>();
            modelBuilder.Entity<DepositAccount>();
            modelBuilder.Entity<CreditAccount>();
            modelBuilder.Entity<PutTransaction>();
            modelBuilder.Entity<WithdrawTransaction>();
            modelBuilder.Entity<TransferTransaction>();
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=datadb;Trusted_Connection=True;");
        }
    }
}