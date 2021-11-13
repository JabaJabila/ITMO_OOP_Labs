using System;

namespace Banks.Transactions
{
    public abstract class TransactionHandler : ITransactionHandler
    {
        private ITransactionHandler _nextHandler;

        public ITransactionHandler SetNext(ITransactionHandler handler)
        {
            _nextHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            return handler;
        }

        public virtual void Handle(Transaction transaction)
        {
            if (_nextHandler != null)
            {
                _nextHandler.Handle(transaction);
            }
        }
    }
}