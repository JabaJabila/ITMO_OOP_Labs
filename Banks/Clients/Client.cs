using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Banks.BankSystem;
using Banks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Banks.Clients
{
    public class Client
    {
        private readonly List<Bank> _banks;
        private readonly List<Bank> _creditAccountChangesPublishers;
        private readonly List<Bank> _debitAccountChangesPublishers;
        private readonly List<Bank> _depositAccountChangesPublishers;

        private Client(string name, string surname)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            _banks = new List<Bank>();
            _creditAccountChangesPublishers = new List<Bank>();
            _debitAccountChangesPublishers = new List<Bank>();
            _depositAccountChangesPublishers = new List<Bank>();
        }

        private Client()
        {
        }

        public delegate void NotificationHandler(string message);
        public event NotificationHandler Notify;

        public int Id { get; private init; }

        [Required]
        public string Name { get; private init; }
        [Required]
        public string Surname { get; private init; }
        public string Address { get; private set; }
        public string Passport { get; private set; }

        [BackingField(nameof(_creditAccountChangesPublishers))]
        public IReadOnlyList<Bank> CreditAccountChangesPublishers => _creditAccountChangesPublishers;

        [BackingField(nameof(_debitAccountChangesPublishers))]
        public IReadOnlyList<Bank> DebitAccountChangesPublishers => _debitAccountChangesPublishers;

        [BackingField(nameof(_depositAccountChangesPublishers))]
        public IReadOnlyList<Bank> DepositAccountChangesPublishers => _depositAccountChangesPublishers;

        [BackingField(nameof(_banks))]
        public IReadOnlyList<Bank> Banks => _banks;

        public string ReactOnChanges(string changesMessage)
        {
            Notify?.Invoke(changesMessage);
            return changesMessage;
        }

        internal void AddBank(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            if (_banks.Contains(bank))
                throw new ClientException($"Client:{Id} already registered in bank:{bank.Id}");

            _banks.Add(bank);
        }

        internal void AddCreditAccountChangesPublisher(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            if (_creditAccountChangesPublishers.Contains(bank))
                throw new ClientException($"Client:{Id} already registered in bank:{bank.Id}");

            _creditAccountChangesPublishers.Add(bank);
        }

        internal void AddDebitAccountChangesPublisher(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            if (_debitAccountChangesPublishers.Contains(bank))
                throw new ClientException($"Client:{Id} already registered in bank:{bank.Id}");

            _debitAccountChangesPublishers.Add(bank);
        }

        internal void AddDepositAccountChangesPublisher(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            if (_depositAccountChangesPublishers.Contains(bank))
                throw new ClientException($"Client:{Id} already registered in bank:{bank.Id}");

            _depositAccountChangesPublishers.Add(bank);
        }

        public class ClientBuilder : IClientBuilder
    {
        private string _name;
        private string _surname;
        private string _address;
        private string _passport;

        public void Reset()
        {
            _name = null;
            _surname = null;
            _address = null;
            _passport = null;
        }

        public void UpdateClientAddress(Client client, string address)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (address == null)
                throw new ArgumentNullException(nameof(address));

            client.Address = address;
        }

        public void UpdateClientPassport(Client client, string passport)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (passport == null)
                throw new ArgumentNullException(nameof(passport));

            client.Passport = passport;
        }

        public void AddName(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void AddSurname(string surname)
        {
            _surname = surname ?? throw new ArgumentNullException(nameof(surname));
        }

        public void AddAddress(string address)
        {
            _address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void AddPassport(string passport)
        {
            _passport = passport ?? throw new ArgumentNullException(nameof(passport));
        }

        public Client GetClient()
        {
            if (_name == null || _surname == null)
                throw new ClientException("Client must have name and surname!");

            var client = new Client(_name, _surname);

            if (_address != null)
                client.Address = _address;

            if (_passport != null)
                client.Passport = _passport;

            Reset();
            return client;
        }
    }
    }
}