using System;
using Shops.Tools;

namespace Shops.Entities
{
    public class Product : IEquatable<Product>
    {
        private static int _productUniqueId = 1;
        public Product(string name)
        {
            Id = _productUniqueId++;
            Name = name ?? throw new ArgumentNullException(nameof(name), $"{nameof(name)} can't be null!");
        }

        public int Id { get; }
        public string Name { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public override bool Equals(object obj)
        {
            return obj is Product product && Equals(product);
        }

        public bool Equals(Product other)
        {
            return other != null
                   && Id == other.Id
                   && Name == other.Name;
        }
    }
}