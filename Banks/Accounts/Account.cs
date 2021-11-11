using System;
using Banks.Clients;
using Banks.Exceptions;
using Banks.Transactions;

namespace Banks.Accounts
{
    public abstract class Account
    {
        private readonly TransactionHandler _transactionHandler;

        internal Account(Client client, decimal starterBalance)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Balance = starterBalance;
            CheckIfClientTrusted();
            _transactionHandler = new PutTransactionHandler();
            _transactionHandler
                .SetNext(new WithdrawTransactionHandler())
                .SetNext(new TransferTransactionHandler());
        }

        protected Account()
        {
            _transactionHandler = new PutTransactionHandler();
            _transactionHandler
                .SetNext(new WithdrawTransactionHandler())
                .SetNext(new TransferTransactionHandler());
        }

        public Client Client { get; private init; }

        public decimal Balance
        {
            get => AccBalance;
            private init
            {
                if (value < 0)
                    throw new AccountException("Starter Balance must be >= 0!");
                AccBalance = value;
            }
        }

        public int Id { get; private init; }
        public double InterestRate { get; internal set; }
        public decimal Commission { get; internal set; }
        public decimal MinusLimit { get; internal set; }
        public bool IsTrusted { get; internal set; }
        public decimal UntrustedLimit { get; internal set; }
        protected decimal AccBalance { get; set; }

        public abstract void CalculateInterest(DateTime dateTime);
        public abstract void ChargeInterest();
        public abstract void ChargeCommission();

        public virtual bool WithdrawAndTransfersAreAvailable()
        {
            return true;
        }

        public PutTransaction StartPutTransaction(decimal amount, TransactionHistory history)
        {
            CheckIfClientTrusted();
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            var transaction = new PutTransaction(this, amount);
            _transactionHandler.Handle(transaction);
            transaction.Execute();
            history.AddTransaction(transaction);
            return transaction;
        }

        public WithdrawTransaction StartWithdrawTransaction(decimal amount, TransactionHistory history)
        {
            CheckIfClientTrusted();

            if (history == null)
                throw new ArgumentNullException(nameof(history));

            var transaction = new WithdrawTransaction(this, amount);
            _transactionHandler.Handle(transaction);
            transaction.Execute();
            history.AddTransaction(transaction);
            return transaction;
        }

        public TransferTransaction StartTransferTransaction(
            decimal amount,
            Account receiver,
            TransactionHistory history)
        {
            CheckIfClientTrusted();
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            if (history == null)
                throw new ArgumentNullException(nameof(history));

            var transaction = new TransferTransaction(this, amount, receiver);
            _transactionHandler.Handle(transaction);
            transaction.Execute();
            history.AddTransaction(transaction);
            return transaction;
        }

        internal void PutMoney(decimal amount)
        {
            AccBalance += amount;
        }

        internal void WithdrawMoney(decimal amount)
        {
            AccBalance -= amount;
        }

        private void CheckIfClientTrusted()
        {
            if (IsTrusted)
                return;

            if (Client.Passport != null && Client.Address != null)
                IsTrusted = true;
        }
    }
}