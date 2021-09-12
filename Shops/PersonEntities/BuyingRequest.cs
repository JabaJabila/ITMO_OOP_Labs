using Shops.ShopEntities;
using Shops.Tools;

namespace Shops.PersonEntities
{
    public class BuyingRequest
    {
        public BuyingRequest(Product product, uint count)
        {
            Product = product ?? throw new ProductException("Wishing product can't be null!");
            if (count == 0)
                throw new ProductException("You can't wish to buy 0 products!");
            Count = count;
        }

        public Product Product { get; }
        public uint Count { get; }
    }
}