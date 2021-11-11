using System;
using System.Collections.Generic;
using Banks.Exceptions;

namespace Banks.Accounts
{
    public class DebitAccountConfig
    {
        private const double Tolerance = 0.01;
        private readonly double _interestRate;
        private readonly decimal _untrustedLimit;

        public DebitAccountConfig(double interestRate, decimal untrustedLimit)
        {
            InterestRate = interestRate;
            UntrustedLimit = untrustedLimit;
        }

        private DebitAccountConfig()
        {
        }

        public int Id { get; private init; }

        public double InterestRate
        {
            get => _interestRate;
            private init
            {
                if (value < 0)
                    throw new AccountException("Interest rate must be >= 0!");
                _interestRate = value;
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

        public static string GetDifference(DebitAccountConfig oldConfig, DebitAccountConfig newConfig)
        {
            var changes = new List<string>();
            if (Math.Abs(oldConfig.InterestRate - newConfig.InterestRate) > Tolerance)
            {
                changes.Add($"New interest rate:" +
                            $"{newConfig.InterestRate}% instead of {oldConfig.InterestRate}%");
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