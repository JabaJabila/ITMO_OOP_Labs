using System.Collections.Generic;
using Banks.Exceptions;

namespace Banks.Accounts
{
    public class CreditAccountConfig
    {
        private readonly decimal _commission;
        private readonly decimal _minusLimit;
        private readonly decimal _untrustedLimit;

        public CreditAccountConfig(decimal commission, decimal minusLimit, decimal untrustedLimit)
        {
            Commission = commission;
            MinusLimit = minusLimit;
            UntrustedLimit = untrustedLimit;
        }

        private CreditAccountConfig()
        {
        }

        public decimal Commission
        {
            get => _commission;
            private init
            {
                if (value < 0)
                    throw new AccountException("Commission must be > 0!");
                _commission = value;
            }
        }

        public int Id { get; private init; }

        public decimal MinusLimit
        {
            get => _minusLimit;
            private init
            {
                if (value > 0)
                    throw new AccountException("Impossible to set limit for credit account >= 0");
                _minusLimit = value;
            }
        }

        public decimal UntrustedLimit
        {
            get => _untrustedLimit;
            private init
            {
                if (value < 0)
                    throw new AccountException("Untrusted Limit must be >= 0!");
                _untrustedLimit = value;
            }
        }

        public static string GetDifference(CreditAccountConfig oldConfig, CreditAccountConfig newConfig)
        {
            var changes = new List<string>();
            if (oldConfig.Commission != newConfig.Commission)
            {
                changes.Add($"New commission amount:" +
                            $"{newConfig.Commission}% instead of {oldConfig.Commission}%");
            }

            if (oldConfig.MinusLimit != newConfig.MinusLimit)
            {
                changes.Add($"New minus limit amount:" +
                            $"{newConfig.MinusLimit}% instead of {oldConfig.MinusLimit}%");
            }

            if (oldConfig.UntrustedLimit != newConfig.UntrustedLimit)
            {
                changes.Add($"New untrusted limit:" +
                            $"{newConfig.UntrustedLimit} instead of {oldConfig.UntrustedLimit}");
            }

            if (changes.Count == 0)
                throw new AccountException("No changes in config!");

            return string.Join('\n', changes);
        }
    }
}