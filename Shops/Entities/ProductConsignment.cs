using Shops.Tools;

namespace Shops.Entities
{
    public class ProductConsignment : BuyingRequest
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
    }
}