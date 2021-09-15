using Shops.Tools;

namespace Shops.Entities
{
    public class Person
    {
        private decimal _balance;
        public Person(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }

        public string Name { get; }
        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0)
                    throw new PersonException("Balance should be >= 0!");
                _balance = value;
            }
        }

        public void PayBill(decimal price)
        {
            if (price < 0)
                throw new PersonException("Price should be >= 0!");
            if (price > Balance)
                throw new PersonException($"Person {Name} doesn't have enough money ({Balance}) to pay {price}!");
            Balance -= price;
        }
    }
}