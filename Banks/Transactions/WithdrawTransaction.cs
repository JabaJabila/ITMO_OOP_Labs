using Banks.Accounts;

namespace Banks.Transactions
{
    public class WithdrawTransaction : Transaction
    {
        public WithdrawTransaction(Account accountFrom, decimal amount)
            : base(accountFrom, amount)
        {
        }

        private WithdrawTransaction()
        {
        }

        public override void Execute()
        {
            AccountFrom.WithdrawMoney(Amount);
        }

        internal override void Undo()
        {
            AccountFrom.PutMoney(Amount);
        }
    }
}