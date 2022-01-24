using System;
using Shops.Tools;

namespace Shops.Entities
{
    public class BuyingRequest : IEquatable<BuyingRequest>
    {
        public BuyingRequest(Product product, uint count)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));

            if (count == 0)
                throw new ProductException("You can't wish to get 0 products!");
            Count = count;
        }

        public Product Product { get; }
        public uint Count { get; internal set; }

        public bool Equals(BuyingRequest other)
        {
            return other != null
                   && Product.Equals(other.Product)
                   && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is BuyingRequest buyingRequest && Equals(buyingRequest);
        }

        public override int GetHashCode()
        {
            return Product.Id;
        }
    }
}