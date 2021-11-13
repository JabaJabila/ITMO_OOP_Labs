using Banks.Exceptions;

namespace Banks.Transactions
{
    public class WithdrawTransactionHandler : TransactionHandler
    {
        public override void Handle(Transaction transaction)
        {
            if (transaction.GetType().Name == nameof(WithdrawTransaction))
            {
                if (!transaction.AccountFrom.IsTrusted &&
                    transaction.Amount > transaction.AccountFrom.UntrustedLimit)
                {
                    throw new TransactionException($"You can't withdraw {transaction.Amount} because" +
                                                   $" {transaction.AccountFrom.UntrustedLimit} " +
                                                   "is limit for untrusted clients!");
                }

                if (!transaction.AccountFrom.WithdrawAndTransfersAreAvailable())
                {
                    throw new TransactionException(
                        $"Withdraws are not available for account:{transaction.AccountFrom.Id}");
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