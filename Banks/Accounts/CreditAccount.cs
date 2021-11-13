using System;
using Banks.Clients;

namespace Banks.Accounts
{
    public class CreditAccount : Account
    {
        internal CreditAccount(
            Client client,
            decimal starterBalance,
            CreditAccountConfig config)
            : base(client, starterBalance)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            Commission = config.Commission;
            MinusLimit = config.MinusLimit;
            UntrustedLimit = config.UntrustedLimit;
        }

        private CreditAccount()
        {
        }

        public override void CalculateInterest(DateTime dateTime)
        {
        }

        public override void ChargeInterest()
        {
        }

        public override void ChargeCommission()
        {
            if (Balance < 0)
                AccBalance -= Commission;
        }
    }
}