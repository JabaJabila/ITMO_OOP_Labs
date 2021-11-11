using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Banks.Exceptions;

namespace Banks.Accounts
{
    public class DepositAccountConfig
    {
        private const double Tolerance = 0.01;
        private readonly decimal _untrustedLimit;
        private readonly double _defaultInterestRate;
        private readonly List<decimal> _borders;
        private readonly List<double> _interests;

        public DepositAccountConfig(double defaultInterestRate, decimal untrustedLimit)
        {
            _borders = new List<decimal>();
            _interests = new List<double>();
            DefaultInterestRate = defaultInterestRate;
            UntrustedLimit = untrustedLimit;
        }

        private DepositAccountConfig()
        {
            _borders = new List<decimal>();
            _interests = new List<double>();
        }

        public delegate void NewInterestRateStageHandler(decimal balanceUpperBorder, double interestRate);
        public event NewInterestRateStageHandler NotifyDepositAccountConfigChanges;

        public int Id { get;  private init; }

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

        public double DefaultInterestRate
        {
            get => _defaultInterestRate;
            private init
            {
                if (value < 0)
                    throw new AccountException("Interest rate must be >= 0!");
                _defaultInterestRate = value;
            }
        }

        public string InterestRateStages
        {
            get
            {
                List<string> borders = _borders
                    .ConvertAll(b => b.ToString(CultureInfo.CurrentCulture));
                List<string> rates = _interests
                    .ConvertAll(r => r.ToString(CultureInfo.CurrentCulture));
                var stages = borders
                    .Zip(rates, (border, rate) => border + " - " + rate).ToList();
                return string.Join('\n', stages);
            }
            private init
            {
                if (value == string.Empty)
                {
                    _borders = new List<decimal>();
                    _interests = new List<double>();
                    return;
                }

                var stages = value.Split('\n').ToList();
                _borders = stages.Select(s => Convert.ToDecimal(s.Split(" - ").First())).ToList();
                _interests = stages.Select(s => Convert.ToDouble(s.Split(" - ").Last())).ToList();
            }
        }

        public static string GetDifference(DepositAccountConfig oldConfig, DepositAccountConfig newConfig)
        {
            var changes = new List<string>();
            if (oldConfig.UntrustedLimit != newConfig.UntrustedLimit)
            {
                changes.Add("New untrusted limit:" +
                            $"{newConfig.UntrustedLimit} instead of {oldConfig.UntrustedLimit}");
            }

            if (Math.Abs(oldConfig.DefaultInterestRate - newConfig.DefaultInterestRate) > Tolerance ||
                oldConfig.InterestRateStages != newConfig.InterestRateStages)
            {
                changes.Add($"New interest rate stages:\n{newConfig.GetAllInterestsInfo()}" +
                            $"\n\nInstead of:\n{oldConfig.GetAllInterestsInfo()}");
            }

            if (changes.Count == 0)
                throw new AccountException("No changes in config!");

            return string.Join('\n', changes);
        }

        public string GetAllInterestsInfo()
        {
            List<string> borders = _borders
                .ConvertAll(b => b.ToString(CultureInfo.CurrentCulture));
            List<string> rates = _interests
                .ConvertAll(r => r.ToString(CultureInfo.CurrentCulture));
            if (_borders.Count > 0)
            {
                var stages = borders
                    .Zip(rates, (border, rate) => "< " + border + " - " + rate + "%").ToList();
                stages.Add(">= " + _borders.Last() + " - " + DefaultInterestRate);
                return string.Join("\n", stages);
            }

            return $"Interest rate for all amounts = {DefaultInterestRate}";
        }

        public void AddInterestSection(decimal balanceUpperBorder, double interestRate)
        {
            if (balanceUpperBorder < 0)
                throw new AccountException("Impossible to set interest section on balance < 0!");

            if (interestRate < 0)
                throw new AccountException("Impossible to set interest rate < 0%!");

            NotifyDepositAccountConfigChanges?.Invoke(balanceUpperBorder, interestRate);

            if (_borders.Count == 0)
            {
                _borders.Add(balanceUpperBorder);
                _interests.Add(interestRate);
                return;
            }

            for (int counter = 0; counter < _borders.Count; counter++)
            {
                if (_borders[counter] == balanceUpperBorder)
                {
                    _interests[counter] = interestRate;
                    break;
                }

                if (_borders[counter] > balanceUpperBorder)
                {
                    _borders.Insert(counter, balanceUpperBorder);
                    _interests.Insert(counter, interestRate);
                    break;
                }

                if (counter == _borders.Count - 1)
                {
                    _borders.Add(balanceUpperBorder);
                    _interests.Add(interestRate);
                    break;
                }
            }
        }

        public double GetCorrectInterestRate(decimal starterBalance)
        {
            if (starterBalance <= 0)
                throw new AccountException("Impossible to get interest for balance <= 0");

            for (int counter = 0; counter < _borders.Count; counter++)
            {
                if (starterBalance < _borders[counter])
                    return _interests[counter];
            }

            return _defaultInterestRate;
        }
    }
}