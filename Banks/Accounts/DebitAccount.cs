using System;
using Banks.Clients;

namespace Banks.Accounts
{
    public class DebitAccount : Account
    {
        private const int MonthsInYear = 12;

        internal DebitAccount(
            Client client,
            decimal starterBalance,
            DebitAccountConfig config)
            : base(client, starterBalance)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            InterestRate = config.InterestRate;
            UntrustedLimit = config.UntrustedLimit;
        }

        private DebitAccount()
        {
        }

        public decimal CurrentInterest { get; private set; }

        public override void CalculateInterest(DateTime dateTime)
        {
            CurrentInterest = Balance * (decimal)(InterestRate / (MonthsInYear *
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
    }
}