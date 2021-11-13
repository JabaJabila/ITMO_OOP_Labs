namespace Banks.Transactions
{
    public interface ITransactionHandler
    {
        ITransactionHandler SetNext(ITransactionHandler handler);
        void Handle(Transaction transaction);
    }
}