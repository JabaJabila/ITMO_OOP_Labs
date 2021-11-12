using System;
using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.Clients;
using Banks.Exceptions;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class BanksTests
    {
        private CentralBank _centralBank;
        private DateSystem _date;

        [SetUp]
        public void SetUp()
        {
            _date = new DateSystem(DateTime.Today);
            _centralBank = new CentralBank(_date);
        }

        [TestCase(
            "Tinkoff",
            "Artem", 
            "Andreev", 
            "Somewhere",
            "Passport")]
        public void CreateBankAndAddClient_BankAndClientCreated(
            string bankName,
            string clientName,
            string clientSurname,
            string clientAddress,
            string clientPassport)
        {
            Bank bank = _centralBank.RegisterNewBank(
                bankName,
                new DebitAccountConfig(1, 100),
                new DepositAccountConfig(1, 100),
                new CreditAccountConfig(50, -1000, 100));
            
            var cbuilder = new Client.ClientBuilder();
            cbuilder.AddName(clientName).AddSurname(clientSurname).AddAddress(clientAddress).AddPassport(clientPassport);
            Client client = cbuilder
                .AddName(clientName)
                .AddSurname(clientSurname)
                .AddAddress(clientAddress)
                .AddPassport(clientPassport)
                .GetClient();
            bank.AddClient(client);
            
            Assert.Contains(bank, _centralBank.Banks.ToArray());
            Assert.Contains(client, bank.Clients.ToArray());
        }

        [TestCase(5, 10000)]
        public void CreateDebitAccount_CheckInterestsAfter1DayAndBalanceAfter1Month(
            double interestRate,
            decimal balance)
        {
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(interestRate, 100),
                new DepositAccountConfig(1, 100),
                new CreditAccountConfig(50, -1000, 100));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder.AddName("Name").AddSurname("Surname").GetClient();
            bank.AddClient(client);
            DebitAccount account = bank.CreateDebitAccount(client, balance);
            _date.SkipDays(1);
            Assert.Less(
                account.Balance * (decimal) interestRate / 
                (12 * DateTime.DaysInMonth(_date.DateTime.Year, _date.DateTime.Month))
                - account.CurrentInterest,
                0.01);
            _date.SkipDays(DateTime.DaysInMonth(_date.DateTime.Year, _date.DateTime.Month));
            Assert.Greater(account.Balance, balance);
        }
        
        [TestCase(50)]
        public void CreateCreditAccount_CommissionForMinusBalance(
            decimal commission)
        {
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(5, 100),
                new DepositAccountConfig(1, 100),
                new CreditAccountConfig(commission, -1000, 100));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder.AddName("Name").AddSurname("Surname").GetClient();
            bank.AddClient(client);
            CreditAccount account = bank.CreateCreditAccount(client, 0);
            account.StartWithdrawTransaction(10, _centralBank.TransactionHistory);
            _date.SkipDays(1);
            Assert.AreEqual(-10 - commission, account.Balance);
        }

        [TestCase(5, 4, 1000)]
        public void CreateSavingDepositAccount_CheckBalanceAndNoWithdraws(
            double defaultInterestRate,
            double additionalInterestRate,
            decimal balance)
        {
            var config = new DepositAccountConfig(defaultInterestRate, 100);
            config.AddInterestSection(balance + 1, additionalInterestRate);
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(5, 100),
                config,
                new CreditAccountConfig(50, -1000, 100));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder
                .AddName("Name")
                .AddSurname("Surname")
                .AddAddress("Address")
                .AddPassport("Passport")
                .GetClient();
            bank.AddClient(client);
            DepositAccount account = bank.CreateDepositAccount(
                client,
                balance,
                _date.DateTime.AddMonths(2));
            Assert.AreEqual(additionalInterestRate, account.InterestRate);
            _date.SkipDays(32);
            Assert.Greater(account.Balance, balance);
            Assert.True(account.IsSaving);
            Assert.Throws<TransactionException>(() =>
            {
                account.StartWithdrawTransaction(balance, _centralBank.TransactionHistory);
            });
        }
        
        [TestCase(5, 4, 1000, 800)]
        public void SubscribeOnDepositAccountChanges_ClientSubscribedAndGetsNotifications(
            double defaultInterestRate,
            double newInterestRate,
            decimal defaultUntrustedLimit,
            decimal newUntrustedLimit)
        {
            var config = new DepositAccountConfig(defaultInterestRate, defaultUntrustedLimit);
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(5, 100),
                config,
                new CreditAccountConfig(50, -1000, 100));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder
                .AddName("Name")
                .AddSurname("Surname")
                .AddAddress("Address")
                .AddPassport("Passport")
                .GetClient();
            bank.AddClient(client);
            DepositAccount account = bank.CreateDepositAccount(
                client,
                100,
                _date.DateTime.AddMonths(2));
            Client.NotificationHandler myHandler = _ => { _date.SkipDays(1); };
            client.Notify += myHandler;
            bank.SubscribeOnDepositAccountChanges(client);
            Assert.Contains(bank, client.DepositAccountChangesPublishers.ToArray());
            Assert.Contains(client, bank.OnDepositAccountChangesSubscribers.ToArray());
            config.AddInterestSection(1000, newInterestRate);
            bank.ChangeDepositAccountConfig(new DepositAccountConfig(defaultInterestRate, newUntrustedLimit));
            Assert.AreEqual(DateTime.Today.AddDays(2), _date.DateTime);
            client.Notify -= myHandler;
        }
        
        [TestCase(100, 200)]
        [TestCase(500, 501)]
        public void TransactionsWithUntrustedAccount_LimitExceeded_ThrowsException(
            decimal untrustedLimit,
            decimal moneyAmount)
        {
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(5, untrustedLimit),
                new DepositAccountConfig(5, untrustedLimit),
                new CreditAccountConfig(50, -1000, untrustedLimit));
            
            var cbuilder = new Client.ClientBuilder();
            Client client1 = cbuilder
                .AddName("Name")
                .AddSurname("Surname")
                .AddAddress("Address")
                .GetClient();
            Client client2 = cbuilder
                .AddName("OtherName")
                .AddSurname("OtherSurname")
                .AddAddress("OtherAddress")
                .AddPassport("Passport")
                .GetClient();
            bank.AddClient(client1);
            bank.AddClient(client2);
            DebitAccount account1 = bank.CreateDebitAccount(
                client1,
                moneyAmount);
            DebitAccount account2 = bank.CreateDebitAccount(
                client2,
                moneyAmount);
            Assert.False(account1.IsTrusted);
            Assert.Throws<TransactionException>(() =>
            {
                account1.StartWithdrawTransaction(moneyAmount, _centralBank.TransactionHistory);
            });
            Assert.Throws<TransactionException>(() =>
            {
                account1.StartPutTransaction(moneyAmount, _centralBank.TransactionHistory);
            });
            Assert.Throws<TransactionException>(() =>
            {
                account1.StartTransferTransaction(moneyAmount, account2, _centralBank.TransactionHistory);
            });
            Assert.Throws<TransactionException>(() =>
            {
                account2.StartTransferTransaction(moneyAmount, account1, _centralBank.TransactionHistory);
            });
        }

        [TestCase(-1000, 1005)]
        [TestCase(-10000, 15000)]
        public void MinusLimitOfCreditAccountExceeded_ThrowsException(
            decimal minusLimit,
            decimal amount)
        {
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(5, amount),
                new DepositAccountConfig(1, amount),
                new CreditAccountConfig(100, minusLimit, amount));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder
                .AddName("Name")
                .AddSurname("Surname")
                .GetClient();
            bank.AddClient(client);
            CreditAccount account = bank.CreateCreditAccount(client, 0);
            
            Assert.Throws<TransactionException>(() =>
            {
                account.StartWithdrawTransaction(amount, _centralBank.TransactionHistory);
            });
        }

        [TestCase(1000, 800)]
        [TestCase(10000, 10)]
        public void MakeTransactionAndCancelIt_TransactionCancelled(
            decimal balance,
            decimal amountToSpend)
        {
            Bank bank = _centralBank.RegisterNewBank(
                "SomeName",
                new DebitAccountConfig(1, amountToSpend),
                new DepositAccountConfig(1, amountToSpend),
                new CreditAccountConfig(50, -1000, amountToSpend));
            
            var cbuilder = new Client.ClientBuilder();
            Client client = cbuilder
                .AddName("Name")
                .AddSurname("Surname")
                .GetClient();
            bank.AddClient(client);
            DebitAccount account = bank.CreateDebitAccount(client, balance);
            WithdrawTransaction transaction = account.StartWithdrawTransaction(amountToSpend, _centralBank.TransactionHistory);
            Assert.AreEqual(balance - amountToSpend, account.Balance);
            Assert.Contains(transaction, _centralBank.TransactionHistory.Transactions);
            _centralBank.TransactionHistory.RemoveTransaction(transaction);
            Assert.AreEqual(balance, account.Balance);
            Assert.IsEmpty(_centralBank.TransactionHistory.Transactions);
        }
    }
}