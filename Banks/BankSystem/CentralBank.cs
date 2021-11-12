using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class CentralBank
    {
        private readonly List<Bank> _banks;
        private DateSystem _dateSystem;

        public CentralBank(DateSystem dateSystem)
        {
            DateSystem = dateSystem ?? throw new ArgumentNullException(nameof(dateSystem));
            _banks = new List<Bank>();
            TransactionHistory = new TransactionHistory();
        }

        private CentralBank()
        {
            _banks = new List<Bank>();
        }

        public int Id { get; private init; }

        public TransactionHistory TransactionHistory { get; private init; }

        [BackingField(nameof(_banks))]
        public IReadOnlyList<Bank> Banks => _banks;

        public DateSystem DateSystem
        {
            get => _dateSystem;
            private init
            {
                _dateSystem = value;
                _dateSystem.NotifyNewDay += NotifyToCalculateInterest;
                _dateSystem.NotifyNewDay += NotifyChargeCommission;
                _dateSystem.NotifyNewMonth += NotifyChargeInterest;
            }
        }

        public void UnsubscribeFromDateTimeEvents()
        {
            DateSystem.NotifyNewDay -= NotifyToCalculateInterest;
            DateSystem.NotifyNewDay -= NotifyChargeCommission;
            DateSystem.NotifyNewMonth -= NotifyChargeInterest;
        }

        public Bank RegisterNewBank(
            string name,
            DebitAccountConfig debitConfig,
            DepositAccountConfig depositConfig,
            CreditAccountConfig creditConfig)
        {
             if (name == null)
                 throw new ArgumentNullException(nameof(name));

             if (debitConfig == null)
                 throw new ArgumentNullException(nameof(debitConfig));

             if (depositConfig == null)
                 throw new ArgumentNullException(nameof(depositConfig));

             if (creditConfig == null)
                 throw new ArgumentNullException(nameof(creditConfig));

             var bank = new Bank(name, debitConfig, depositConfig, creditConfig);
             _banks.Add(bank);

             return bank;
        }

        private void NotifyToCalculateInterest(DateTime date)
        {
            _banks.ForEach(bank => bank.CalculateInterests(date));
        }

        private void NotifyChargeInterest(DateTime date)
        {
            _banks.ForEach(bank => bank.ChargeInterests());
        }

        private void NotifyChargeCommission(DateTime date)
        {
            _banks.ForEach(bank => bank.ChargeCommissions());
        }
    }
}