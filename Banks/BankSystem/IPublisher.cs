using Banks.Accounts;
using Banks.Clients;

namespace Banks.BankSystem
{
    public interface IPublisher
    {
        void CreditAccountChangesNotify(CreditAccountConfig oldConfig, CreditAccountConfig newConfig);
        void DebitAccountChangesNotify(DebitAccountConfig oldConfig, DebitAccountConfig newConfig);
        void DepositAccountChangesNotify(DepositAccountConfig oldConfig, DepositAccountConfig newConfig);
        void DepositAccountChangesNotify(decimal balanceUpperBorder, double interestRate);
        void SubscribeOnCreditAccountChanges(Client client);
        void SubscribeOnDebitAccountChanges(Client client);
        void SubscribeOnDepositAccountChanges(Client client);
    }
}