using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Banks.Accounts;
using Banks.Clients;
using Banks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class Bank : IPublisher
    {
        private readonly List<Client> _clients;
        private readonly List<Account> _accounts;
        private readonly List<Client> _onCreditAccountChangesSubscribers;
        private readonly List<Client> _onDebitAccountChangesSubscribers;
        private readonly List<Client> _onDepositAccountChangesSubscribers;
        private DepositAccountConfig _depositAccountConfig;

        internal Bank(
            string name,
            DebitAccountConfig debitAccountConfig,
            DepositAccountConfig depositAccountConfig,
            CreditAccountConfig creditAccountConfig)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DebitAccountConfig = debitAccountConfig
                                 ?? throw new ArgumentNullException(nameof(debitAccountConfig));
            DepositAccountConfig = depositAccountConfig
                                   ?? throw new ArgumentNullException(nameof(depositAccountConfig));
            CreditAccountConfig = creditAccountConfig
                                  ?? throw new ArgumentNullException(nameof(creditAccountConfig));

            _clients = new List<Client>();
            _accounts = new List<Account>();
            _onCreditAccountChangesSubscribers = new List<Client>();
            _onDebitAccountChangesSubscribers = new List<Client>();
            _onDepositAccountChangesSubscribers = new List<Client>();
        }

        private Bank()
        {
            _clients = new List<Client>();
            _accounts = new List<Account>();
            _onCreditAccountChangesSubscribers = new List<Client>();
            _onDebitAccountChangesSubscribers = new List<Client>();
            _onDepositAccountChangesSubscribers = new List<Client>();
        }

        public int Id { get; private init; }
        [Required]
        public string Name { get; private init; }
        public DebitAccountConfig DebitAccountConfig { get; private set; }

        public DepositAccountConfig DepositAccountConfig
        {
            get => _depositAccountConfig;
            private set
            {
                if (_depositAccountConfig != null)
                    UnsubscribeFromDepositAccountConfigEvents();
                _depositAccountConfig = value;
                _depositAccountConfig.NotifyDepositAccountConfigChanges += DepositAccountChangesNotify;
            }
        }

        public CreditAccountConfig CreditAccountConfig { get; private set; }
        [BackingField(nameof(_clients))]
        public IReadOnlyList<Client> Clients => _clients;
        [BackingField(nameof(_accounts))]
        public IReadOnlyList<Account> Accounts => _accounts;
        [BackingField(nameof(_onCreditAccountChangesSubscribers))]
        public IReadOnlyList<Client> OnCreditAccountChangesSubscribers => _onCreditAccountChangesSubscribers;
        [BackingField(nameof(_onDebitAccountChangesSubscribers))]
        public IReadOnlyList<Client> OnDebitAccountChangesSubscribers => _onDebitAccountChangesSubscribers;
        [BackingField(nameof(_onDepositAccountChangesSubscribers))]
        public IReadOnlyList<Client> OnDepositAccountChangesSubscribers => _onDepositAccountChangesSubscribers;

        public void UnsubscribeFromDepositAccountConfigEvents()
        {
            DepositAccountConfig.NotifyDepositAccountConfigChanges -= DepositAccountChangesNotify;
        }

        public Client AddClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (_clients.Contains(client))
                throw new BankException("Client already registered in this bank!");

            _clients.Add(client);
            client.AddBank(this);
            return client;
        }

        public void ChangeDebitAccountConfig(DebitAccountConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            DebitAccountChangesNotify(DebitAccountConfig, config);
            DebitAccountConfig = config;
        }

        public void ChangeCreditAccountConfig(CreditAccountConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            CreditAccountChangesNotify(CreditAccountConfig, config);
            CreditAccountConfig = config;
        }

        public void ChangeDepositAccountConfig(DepositAccountConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            DepositAccountChangesNotify(DepositAccountConfig, config);
            DepositAccountConfig = config;
        }

        public DebitAccount CreateDebitAccount(Client client, decimal starterBalance)
        {
            ValidateForNewAccount(client, starterBalance);

            var account = new DebitAccount(
                client,
                starterBalance,
                DebitAccountConfig);
            _accounts.Add(account);
            return account;
        }

        public DepositAccount CreateDepositAccount(Client client, decimal starterBalance, DateTime savingUntil)
        {
            ValidateForNewAccount(client, starterBalance);

            var account = new DepositAccount(
                client,
                starterBalance,
                DepositAccountConfig,
                savingUntil);
            _accounts.Add(account);
            return account;
        }

        public CreditAccount CreateCreditAccount(Client client, decimal starterBalance)
        {
            ValidateForNewAccount(client, starterBalance);

            var account = new CreditAccount(
                client,
                starterBalance,
                CreditAccountConfig);
            _accounts.Add(account);
            return account;
        }

        public void CalculateInterests(DateTime dateTime)
        {
            _accounts.ForEach(account => account.CalculateInterest(dateTime));
        }

        public void ChargeInterests()
        {
            _accounts.ForEach(account => account.ChargeInterest());
        }

        public void ChargeCommissions()
        {
            _accounts.ForEach(account => account.ChargeCommission());
        }

        public void CreditAccountChangesNotify(CreditAccountConfig oldConfig, CreditAccountConfig newConfig)
        {
            _onCreditAccountChangesSubscribers
                .ForEach(client => client.ReactOnChanges(
                    CreditAccountConfig.GetDifference(oldConfig, newConfig).ToArray()));
        }

        public void DebitAccountChangesNotify(DebitAccountConfig oldConfig, DebitAccountConfig newConfig)
        {
            _onDebitAccountChangesSubscribers
                .ForEach(client => client.ReactOnChanges(
                    DebitAccountConfig.GetDifference(oldConfig, newConfig).ToArray()));
        }

        public void DepositAccountChangesNotify(DepositAccountConfig oldConfig, DepositAccountConfig newConfig)
        {
            _onDepositAccountChangesSubscribers
                .ForEach(client => client.ReactOnChanges(
                    DepositAccountConfig.GetDifference(oldConfig, newConfig).ToArray()));
        }

        public void DepositAccountChangesNotify(decimal balanceUpperBorder, double interestRate)
        {
            _onDepositAccountChangesSubscribers.ForEach(client => client.ReactOnChanges(
                $"New interest rate stage:\n< {balanceUpperBorder} - {interestRate}%"));
        }

        public void SubscribeOnCreditAccountChanges(Client client)
        {
            ValidateClient(client);

            if (_onCreditAccountChangesSubscribers.Contains(client))
            {
                throw new BankException(
                    "Client has already subscribed on credit card changes in this bank!");
            }

            client.AddCreditAccountChangesPublisher(this);
            _onCreditAccountChangesSubscribers.Add(client);
        }

        public void SubscribeOnDebitAccountChanges(Client client)
        {
            ValidateClient(client);

            if (_onDebitAccountChangesSubscribers.Contains(client))
            {
                throw new BankException(
                    "Client has already subscribed on credit card changes in this bank!");
            }

            client.AddDebitAccountChangesPublisher(this);
            _onDebitAccountChangesSubscribers.Add(client);
        }

        public void SubscribeOnDepositAccountChanges(Client client)
        {
            ValidateClient(client);

            if (_onDebitAccountChangesSubscribers.Contains(client))
            {
                throw new BankException(
                    "Client has already subscribed on deposit card changes in this bank!");
            }

            client.AddDepositAccountChangesPublisher(this);
            _onDepositAccountChangesSubscribers.Add(client);
        }

        private void ValidateClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (!_clients.Contains(client))
                throw new BankException("Client isn't registered in this bank!");
        }

        private void ValidateForNewAccount(Client client, decimal starterBalance)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (starterBalance < 0)
                throw new BankException("Impossible to start account with balance < 0");

            if (!_clients.Contains(client))
                throw new BankException($"Client {client.Id} isn't client of this bank!");
        }
    }
}