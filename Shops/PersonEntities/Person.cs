using Shops.Tools;

namespace Shops.PersonEntities
{
    public class Person
    {
        public Person(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }

        public string Name { get; }
        public decimal Balance { get; private set; }

        public void PayBill(decimal price)
        {
            if (price <= 0)
                throw new PersonException("Price should be above zero");
            if (price > Balance)
                throw new PersonException($"Person {Name} doesn't have enough money ({Balance}) to pay {price}!");
            Balance -= price;
        }
    }
}