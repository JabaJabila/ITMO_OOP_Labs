using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private readonly List<ProductConsignment> _productsInStock;

        internal Shop(int id, string name, string address)
        {
            Id = id;
            Name = name ?? throw new ShopException("Shop must has it's own name");
            Address = address ?? throw new ShopException("Shop must has it's own address");
            _productsInStock = new List<ProductConsignment>();
        }

        public int Id { get; }
        public string Name { get; }
        public string Address { get; }
        public IReadOnlyList<ProductConsignment> Assortment => _productsInStock;

        public void AddProducts(List<ProductConsignment> productsToAdd)
        {
            foreach (ProductConsignment productConsignment in productsToAdd)
            {
                ProductConsignment existingProduct = FindProductConsignment(productConsignment.Product);
                if (existingProduct == null)
                {
                    _productsInStock.Add(productConsignment);
                    continue;
                }

                existingProduct.Count += productConsignment.Count;
            }
        }

        public ProductConsignment FindProductConsignment(Product product)
        {
            return _productsInStock.FirstOrDefault(productConsignment => productConsignment.Product.Equals(product));
        }

        public void SetPrice(Product product, decimal price)
        {
            ProductConsignment productConsignment = FindProductConsignment(product);
            if (productConsignment == null)
                throw new ShopException($"Product {product.Name} can't be found in shop {Name}");
            productConsignment.Price = price;
        }

        public void Buy(Person person, List<BuyingRequest> shoppingList)
        {
            decimal totalPrice = 0M;
            var productsToBuy = new List<ProductConsignment>();
            foreach (BuyingRequest buyingRequest in shoppingList)
            {
                ProductConsignment currentConsignment = FindProductConsignment(buyingRequest.Product);
                if (currentConsignment == null)
                    throw new ShopException($"There is no {buyingRequest.Product.Name} in this shop!");
                if (currentConsignment.Count < buyingRequest.Count)
                {
                    throw new ShopException(
                        $"There is no {buyingRequest.Product.Name} in the amount of {buyingRequest.Count}!" +
                        $" Only {currentConsignment.Count} has left!");
                }

                productsToBuy.Add(currentConsignment);
                totalPrice += currentConsignment.Price * buyingRequest.Count;
            }

            person.PayBill(totalPrice);
            for (int positionNumber = 0; positionNumber < shoppingList.Count; ++positionNumber)
                productsToBuy[positionNumber].Count -= shoppingList[positionNumber].Count;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var shop = obj as Shop;
            if (shop == null)
                return false;
            return shop.Id == Id && shop.Name == Name && shop.Address == Address;
        }
    }
}