using System;
using Shops.Tools;

namespace Shops.Entities
{
    public class ProductConsignment : BuyingRequest, IEquatable<ProductConsignment>
    {
        private decimal _price;
        public ProductConsignment(Product product, uint count, decimal price)
            : base(product, count)
        {
            Price = price;
        }

        public decimal Price
        {
            get => _price;
            internal set
            {
                if (value <= 0)
                    throw new ProductException("Product price must be > 0");
                _price = value;
            }
        }

        public bool Equals(ProductConsignment other)
        {
            return other != null
                   && base.Equals(other)
                   && _price == other._price;
        }

        public override bool Equals(object obj)
        {
            return obj is ProductConsignment productConsignment && Equals(productConsignment);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}