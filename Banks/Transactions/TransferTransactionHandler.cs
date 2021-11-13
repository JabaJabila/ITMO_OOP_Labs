using Banks.Exceptions;

namespace Banks.Transactions
{
    public class TransferTransactionHandler : TransactionHandler
    {
        public override void Handle(Transaction transaction)
        {
            if (transaction.GetType().Name == nameof(TransferTransaction))
            {
                if (!transaction.AccountFrom.IsTrusted &&
                    transaction.Amount > transaction.AccountFrom.UntrustedLimit)
                {
                    throw new TransactionException($"You can't transfer {transaction.Amount} because" +
                                                   $" {transaction.AccountFrom.UntrustedLimit} " +
                                                   "is limit for untrusted clients!");
                }

                if (!transaction.AccountTo.IsTrusted &&
                    transaction.Amount > transaction.AccountTo.UntrustedLimit)
                {
                    throw new TransactionException($"You can't transfer {transaction.Amount} because" +
                                                   $" {transaction.AccountFrom.UntrustedLimit} " +
                                                   "is limit for untrusted account you want transfer to!");
                }

                if (!transaction.AccountFrom.WithdrawAndTransfersAreAvailable())
                {
                    throw new TransactionException(
                        $"Transfers are not available for account:{transaction.AccountFrom.Id}");
                }

                if (transaction.AccountFrom.Balance - transaction.Amount < transaction.AccountFrom.MinusLimit)
                {
                    throw new TransactionException($"No enough money ({transaction.Amount}) " +
                                                   $"on balance ({transaction.AccountFrom.Balance}) with" +
                                                   $"minus limit - {transaction.AccountFrom.MinusLimit}");
                }

                return;
            }

            base.Handle(transaction);
        }
    }
}