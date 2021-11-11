using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Banks.Accounts;
using Banks.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.BankSystem
{
    public class CentralBank
    {
        private readonly List<Bank> _banks;
        private readonly DateSystem _date;

        public CentralBank(DateSystem dateSystem)
        {
            Date = dateSystem;
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

        [Required]
        public DateSystem Date
        {
            get => _date;
            private init
            {
                _date = value ?? throw new ArgumentNullException(nameof(value));
                _date.NotifyNewDay += NotifyToCalculateInterest;
                _date.NotifyNewDay += NotifyChargeCommission;
                _date.NotifyNewMonth += NotifyChargeInterest;
            }
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

             var bank = new Bank(name, debitConfig, depositConfig, creditConfig, TransactionHistory);
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