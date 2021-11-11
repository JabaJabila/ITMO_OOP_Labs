using System;
using Banks.Accounts;
using Banks.Exceptions;

namespace Banks.Transactions
{
    public class TransferTransaction : Transaction
    {
        public TransferTransaction(Account accountFrom, decimal amount, Account accountTo)
            : base(accountFrom, amount)
        {
            if (accountTo == null)
                throw new ArgumentNullException(nameof(accountTo));

            if (accountTo == AccountFrom)
                throw new TransactionException("Impossible to transfer money to same account");

            AccountTo = accountTo;
        }

        private TransferTransaction()
        {
        }

        public override void Execute()
        {
            AccountFrom.WithdrawMoney(Amount);
            AccountTo.PutMoney(Amount);
        }

        internal override void Undo()
        {
            AccountTo.WithdrawMoney(Amount);
            AccountFrom.PutMoney(Amount);
        }
    }
}