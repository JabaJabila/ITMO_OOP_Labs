using Shops.Tools;

namespace Shops.ShopEntities
{
    public class ProductConsignment
    {
        private decimal _price;
        public ProductConsignment(Product product, uint count, decimal price)
        {
            Product = product ?? throw new ProductException("Product in consignment can't be null");
            Count = count;
            Price = price;
        }

        public Product Product { get; }
        public uint Count { get; internal set; }
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