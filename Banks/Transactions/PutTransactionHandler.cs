using Banks.Exceptions;

namespace Banks.Transactions
{
    public class PutTransactionHandler : TransactionHandler
    {
        public override void Handle(Transaction transaction)
        {
            if (transaction.GetType().Name == nameof(PutTransaction))
            {
                if (!transaction.AccountFrom.IsTrusted &&
                    transaction.Amount > transaction.AccountFrom.UntrustedLimit)
                {
                    throw new TransactionException($"You can't put {transaction.Amount} because" +
                                                   $" {transaction.AccountFrom.UntrustedLimit} is limit! " +
                                                   "for untrusted clients!");
                }

                return;
            }

            base.Handle(transaction);
        }
    }
}