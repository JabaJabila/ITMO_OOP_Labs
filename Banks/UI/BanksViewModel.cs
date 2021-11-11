using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Banks.Accounts;
using Banks.BankSystem;
using Banks.Clients;
using Banks.EntityFrameworkStuff;
using Banks.Exceptions;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.UI
{
    public sealed class BanksViewModel : IDisposable
    {
        private readonly BanksContext _db;
        private readonly IClientBuilder _builder;
        private CentralBank _currentCentralBank;

        public BanksViewModel()
        {
            _db = new BanksContext();
            _builder = new Client.ClientBuilder();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        public IReadOnlyCollection<string> GetCentralBanksInfo()
        {
            var centralBanks = _db.CentralBanks
                .Include(cb => cb.TransactionHistory).ThenInclude(history => history.Transactions)
                .Include(cb => cb.Date)
                .Include(cb => cb.Banks).ThenInclude(b => b.Clients)
                .Include(cb => cb.Banks).ThenInclude(b => b.CreditAccountConfig)
                .Include(cb => cb.Banks).ThenInclude(b => b.DebitAccountConfig)
                .Include(cb => cb.Banks).ThenInclude(b => b.DepositAccountConfig)
                .Include(cb => cb.Banks).ThenInclude(b => b.OnDebitAccountChangesSubscribers)
                .Include(cb => cb.Banks).ThenInclude(b => b.OnDepositAccountChangesSubscribers)
                .Include(cb => cb.Banks).ThenInclude(b => b.OnCreditAccountChangesSubscribers)
                .Include(cb => cb.Banks).ThenInclude(b => b.Accounts)
                .ToList();

            var ids = centralBanks.Select(cb => cb.Id.ToString()).ToList();
            var dates = centralBanks
                .Select(cb => cb.Date.DateTime.ToString(CultureInfo.CurrentCulture))
                .ToList();
            return ids.Zip(dates, (id, date) => id + ": current date = " + date).ToList();
        }

        public void StartNewCentralBank()
        {
            _currentCentralBank = new CentralBank(new DateSystem(DateTime.Now));
            _db.CentralBanks.Add(_currentCentralBank);
        }

        public void FindCentralBankById(int id)
        {
            _currentCentralBank = _db.CentralBanks.FirstOrDefault(cb => cb.Id == id);
            if (_currentCentralBank == null)
                throw new BankException($"Central bank with id={id} doesn't exists");
        }

        public void SkipDays(int days)
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            _currentCentralBank.Date.SkipDays(days);
            _db.Update(_currentCentralBank);
            _db.SaveChanges();
        }

        public IReadOnlyCollection<string> GetTransactionsInfo()
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            var ids = _currentCentralBank.TransactionHistory.Transactions
                .Select(transaction => transaction.Id.ToString()).ToList();
            var types = _currentCentralBank.TransactionHistory.Transactions
                .Select(transaction => transaction.GetType().Name).ToList();
            var from = _currentCentralBank.TransactionHistory.Transactions
                .Select(transaction => transaction.AccountFrom.Id).ToList();
            var to = _currentCentralBank.TransactionHistory.Transactions
                .Select(transaction => transaction.AccountTo?.Id).ToList();
            var sum = _currentCentralBank.TransactionHistory.Transactions
                .Select(transaction => transaction.Amount.ToString(CultureInfo.CurrentCulture)).ToList();

            var info = ids.Zip(types, (id, type) => id + ": " + type).ToList();
            info = info.Zip(from, (res, idFrom) => res + " from id=" + idFrom).ToList();
            info = info.Zip(to, (res, idTo) => res + " to id=" + idTo).ToList();
            return info.Zip(sum, (res, amount) => res + " amount=" + amount).ToList();
        }

        public void CancelTransaction(int transactionId)
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            Transaction transaction = _currentCentralBank.TransactionHistory.Transactions
                .FirstOrDefault(trans => trans.Id == transactionId);

            _currentCentralBank.TransactionHistory.RemoveTransaction(transaction);
            _db.SaveChanges();
        }

        public IReadOnlyCollection<string> GetBanksInfo()
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            var ids = _currentCentralBank.Banks.Select(bank => bank.Id.ToString()).ToList();
            var names = _currentCentralBank.Banks.Select(bank => bank.Name).ToList();
            return ids.Zip(names, (id, name) => id + ": " + name).ToList();
        }

        public int AddNewBank(
            string name,
            decimal untrustedLimit,
            double debitInterest,
            decimal commission,
            decimal minusLimit,
            double depositInterest,
            List<decimal> borders,
            List<double> rates)
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            var debitConf = new DebitAccountConfig(debitInterest, untrustedLimit);
            var depositConf = new DepositAccountConfig(depositInterest, untrustedLimit);
            var creditConf = new CreditAccountConfig(commission, minusLimit, untrustedLimit);

            for (int counter = 0; counter < borders.Count; counter++)
            {
                depositConf.AddInterestSection(borders[counter], rates[counter]);
            }

            Bank bank = _currentCentralBank.RegisterNewBank(
                name,
                debitConf,
                depositConf,
                creditConf);

            _db.Add(debitConf);
            _db.Add(depositConf);
            _db.Add(creditConf);
            _db.Add(bank);
            _db.SaveChanges();
            return bank.Id;
        }

        public void ChangeBankConfigs(
            int bankId,
            decimal untrustedLimit,
            double debitInterest,
            decimal commission,
            decimal minusLimit,
            double depositInterest,
            List<decimal> borders,
            List<double> rates)
        {
            var debitConf = new DebitAccountConfig(debitInterest, untrustedLimit);
            var depositConf = new DepositAccountConfig(depositInterest, untrustedLimit);
            var creditConf = new CreditAccountConfig(commission, minusLimit, untrustedLimit);

            for (int counter = 0; counter < borders.Count; counter++)
                depositConf.AddInterestSection(borders[counter], rates[counter]);

            Bank bank = GetBankById(bankId);

            bank.ChangeDepositAccountConfig(depositConf);
            bank.ChangeCreditAccountConfig(creditConf);
            bank.ChangeDebitAccountConfig(debitConf);
            _db.Add(debitConf);
            _db.Add(depositConf);
            _db.Add(creditConf);
            _db.SaveChanges();
        }

        public IReadOnlyCollection<string> GetClientsInfo(int bankId)
        {
            Bank bank = GetBankById(bankId);

            var ids = bank.Clients.Select(client => client.Id.ToString()).ToList();
            var names = bank.Clients.Select(client => client.Name).ToList();
            var surnames = bank.Clients.Select(client => client.Surname).ToList();
            names = names.Zip(surnames, (name, surname) => name + " " + surname).ToList();
            return ids.Zip(names, (id, name) => id + ": " + name).ToList();
        }

        public IReadOnlyCollection<string> GetAccountsOfBankInfo(int bankId)
        {
            Bank bank = GetBankById(bankId);

            var ids = bank.Accounts.Select(acc => acc.Id.ToString()).ToList();
            var clientIds = bank.Accounts.Select(acc => acc.Client.Id).ToList();
            var types = bank.Accounts.Select(acc => acc.GetType().Name).ToList();
            var balances = bank.Accounts
                .Select(acc => acc.Balance
                    .ToString(CultureInfo.CurrentCulture))
                .ToList();
            var res = ids.Zip(clientIds, (id, cId) => id + ": clientId=" + cId).ToList();
            res = res.Zip(types, (info, type) => info + " type=" + type).ToList();
            return res.Zip(balances, (info, balance) => info + " balance=" + balance).ToList();
        }

        public int CreateClient(int bankId, string name, string surname)
        {
            Bank bank = GetBankById(bankId);
            _builder.Reset();
            _builder.AddName(name);
            _builder.AddSurname(surname);
            Client client = _builder.GetClient();
            _db.Add(client);
            bank.AddClient(client);
            _db.SaveChanges();
            return client.Id;
        }

        public void CreateCreditAccount(int bankId, int clientId, decimal balance)
        {
            Bank bank = GetBankById(bankId);
            Client client = GetClientById(clientId);

            Account acc = bank.CreateCreditAccount(client, balance);
            _db.Add(acc);
            _db.SaveChanges();
        }

        public void CreateDebitAccount(int bankId, int clientId, decimal balance)
        {
            Bank bank = GetBankById(bankId);
            Client client = GetClientById(clientId);

            Account acc = bank.CreateDebitAccount(client, balance);
            _db.Add(acc);
            _db.SaveChanges();
        }

        public void CreateDepositAccount(int bankId, int clientId, decimal balance, int daysToSave)
        {
            Bank bank = GetBankById(bankId);
            Client client = GetClientById(clientId);

            Account acc = bank.CreateDepositAccount(
                client,
                balance,
                _currentCentralBank.Date.DateTime.AddDays(daysToSave));
            _db.Add(acc);
            _db.SaveChanges();
        }

        public string GetClientInfo(int clientId)
        {
            Client client = GetClientById(clientId);

            string info = $"{client.Id.ToString()}: {client.Name} {client.Surname}";
            info += $"\nAddress: {client.Address ?? "<null>"}";
            info += $"\nPassport: {client.Passport ?? "<null>"}";
            return info;
        }

        public string GetAccountInfo(int accountId)
        {
            Account account = GetAccountById(accountId);

            string info = $"{account.Id.ToString()}: ClientId={account.Client.Id}: " +
                          $"{account.Client.Name} {account.Client.Surname}";
            info += $"\nType: {account.GetType().Name}";
            info += $"\nBalance: {account.Balance}";
            info += account.IsTrusted ? "\nTRUSTED" : "\nUNTRUSTED";
            return info;
        }

        public IReadOnlyCollection<string> GetAllAccountsInfo()
        {
            if (_currentCentralBank == null)
                throw new NullReferenceException("Central bank wasn't chosen!");

            var bankIds = _currentCentralBank.Banks
                .Select(bank => bank.Id)
                .ToList();

            var info = new List<string>();
            foreach (int id in bankIds)
                info.AddRange(GetAccountsOfBankInfo(id));

            return info;
        }

        public void ChangePassport(int clientId, string passport)
        {
            Client client = _db.Clients.FirstOrDefault(client => client.Id == clientId);
            if (client == null)
                throw new BankException($"No clients found with id:{clientId}");

            _builder.UpdateClientPassport(client, passport);
            _db.SaveChanges();
        }

        public void ChangeAddress(int clientId, string address)
        {
            Client client = _db.Clients.FirstOrDefault(client => client.Id == clientId);
            if (client == null)
                throw new BankException($"No clients found with id:{clientId}");

            _builder.UpdateClientAddress(client, address);
            _db.SaveChanges();
        }

        public void SubscribeOnChanges(int clientId, int bankId, int typeId)
        {
            Bank bank = GetBankById(bankId);
            Client client = GetClientById(clientId);

            switch (typeId)
            {
                case 1:
                    bank.SubscribeOnCreditAccountChanges(client);
                    break;
                case 2:
                    bank.SubscribeOnDebitAccountChanges(client);
                    break;
                case 3:
                    bank.SubscribeOnDepositAccountChanges(client);
                    break;
                default: throw new BankException("Unknown type of account!");
            }

            _db.SaveChanges();
        }

        public void PutMoney(int accountId, decimal amountToPut)
        {
            Account account = GetAccountById(accountId);
            Transaction transaction = account.StartPutTransaction(
                amountToPut,
                _currentCentralBank.TransactionHistory);
            _db.Add(transaction);
            _db.SaveChanges();
        }

        public void WithdrawMoney(int accountId, decimal amountToWithdraw)
        {
            Account account = GetAccountById(accountId);
            Transaction transaction = account.StartWithdrawTransaction(
                amountToWithdraw,
                _currentCentralBank.TransactionHistory);
            _db.Add(transaction);
            _db.SaveChanges();
        }

        public void TransferMoney(int accFromId, int accToId, decimal amountToTransfer)
        {
            Account accountFrom = GetAccountById(accFromId);
            Account accountTo = GetAccountById(accToId);

            Transaction transaction = accountFrom.StartTransferTransaction(
                amountToTransfer,
                accountTo,
                _currentCentralBank.TransactionHistory);
            _db.Add(transaction);
            _db.SaveChanges();
        }

        private Account GetAccountById(int accountId)
        {
            if (_currentCentralBank == null)
                throw new BankException("Central bak wasn't chosen!");

            Account account = _db.Accounts.FirstOrDefault(account => account.Id == accountId);
            if (account == null)
                throw new BankException($"No accounts found with id:{accountId}");

            return account;
        }

        private Bank GetBankById(int bankId)
        {
            if (_currentCentralBank == null)
                throw new BankException("Central bak wasn't chosen!");

            Bank bank = _currentCentralBank.Banks.FirstOrDefault(bank => bank.Id == bankId);
            if (bank == null)
                throw new BankException($"No banks found with id:{bankId}");

            return bank;
        }

        private Client GetClientById(int clientId)
        {
            if (_currentCentralBank == null)
                throw new BankException("Central bak wasn't chosen!");

            Client client = _db.Clients.FirstOrDefault(client => client.Id == clientId);
            if (client == null)
                throw new BankException($"No clients found with id:{clientId}");

            return client;
        }
    }
}