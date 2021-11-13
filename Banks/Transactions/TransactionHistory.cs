using System;
using System.Collections.Generic;
using Banks.Exceptions;

namespace Banks.Transactions
{
    public class TransactionHistory
    {
        public TransactionHistory()
        {
            Transactions = new List<Transaction>();
        }

        public int Id { get; private init; }
        public List<Transaction> Transactions { get; private init; }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            Transactions.Add(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (!Transactions.Remove(transaction))
                throw new TransactionException($"Transaction:{transaction.Id} not in the history!");

            transaction.Undo();
        }
    }
}