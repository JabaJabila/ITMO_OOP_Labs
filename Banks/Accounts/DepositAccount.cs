using System;
using Banks.Clients;

namespace Banks.Accounts
{
    public class DepositAccount : Account
    {
        private const int MonthsInYear = 12;

        internal DepositAccount(
            Client client,
            decimal starterBalance,
            DepositAccountConfig config,
            DateTime savingUntil)
            : base(client, starterBalance)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            InterestRate = config.GetCorrectInterestRate(starterBalance);
            UntrustedLimit = config.UntrustedLimit;
            SavingUntil = savingUntil;
        }

        private DepositAccount()
        {
        }

        public DateTime SavingUntil { get; private init; }

        public bool IsSaving { get; private set; }
        public decimal CurrentInterest { get; private set; }

        public override bool WithdrawAndTransfersAreAvailable()
        {
            return !IsSaving;
        }

        public override void CalculateInterest(DateTime dateTime)
        {
            CheckIfSaving(dateTime);
            CurrentInterest = AccBalance * (decimal)(InterestRate / (MonthsInYear *
                                                                     DateTime.DaysInMonth(dateTime.Year, dateTime.Month)));
        }

        public override void ChargeInterest()
        {
            AccBalance += CurrentInterest;
            CurrentInterest = 0;
        }

        public override void ChargeCommission()
        {
        }

        private void CheckIfSaving(DateTime dateTime)
        {
            if (DateTime.Compare(dateTime, SavingUntil) > 0) return;
            if (IsSaving)
                ChargeInterest();

            IsSaving = true;
        }
    }
}