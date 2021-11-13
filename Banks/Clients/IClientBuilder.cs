namespace Banks.Clients
{
    public interface IClientBuilder
    {
        IClientBuilder Reset();
        IClientBuilder UpdateClientAddress(Client client, string address);
        IClientBuilder UpdateClientPassport(Client client, string passport);
        IClientBuilder AddName(string name);
        IClientBuilder AddSurname(string surname);
        IClientBuilder AddAddress(string address);
        IClientBuilder AddPassport(string passport);
        Client GetClient();
    }
}