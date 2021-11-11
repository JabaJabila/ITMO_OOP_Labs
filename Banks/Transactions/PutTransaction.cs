using Banks.Accounts;

namespace Banks.Transactions
{
    public class PutTransaction : Transaction
    {
        public PutTransaction(Account accountFrom, decimal amount)
            : base(accountFrom, amount)
        {
        }

        private PutTransaction()
        {
        }

        public override void Execute()
        {
            AccountFrom.PutMoney(Amount);
        }

        internal override void Undo()
        {
            AccountFrom.WithdrawMoney(Amount);
        }
    }
}