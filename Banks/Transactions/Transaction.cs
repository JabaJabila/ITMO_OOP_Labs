using System;
using Banks.Accounts;

namespace Banks.Transactions
{
    public abstract class Transaction
    {
        internal Transaction(Account accountFrom, decimal amount)
        {
            AccountFrom = accountFrom ?? throw new ArgumentNullException(nameof(accountFrom));
            Amount = amount;
        }

        protected Transaction()
        {
        }

        public int Id { get; private init; }
        public Account AccountFrom { get; private init; }
        public Account AccountTo { get; protected init; }
        public decimal Amount { get; private init; }

        public abstract void Execute();
        internal abstract void Undo();
    }
}