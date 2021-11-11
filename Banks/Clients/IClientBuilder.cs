namespace Banks.Clients
{
    public interface IClientBuilder
    {
        void Reset();
        void UpdateClientAddress(Client client, string address);
        void UpdateClientPassport(Client client, string passport);
        void AddName(string name);
        void AddSurname(string surname);
        void AddAddress(string address);
        void AddPassport(string passport);
        Client GetClient();
    }
}