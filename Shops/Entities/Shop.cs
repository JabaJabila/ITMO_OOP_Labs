using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop : IEquatable<Shop>
    {
        private static int _shopUniqueId = 1;
        private readonly List<ProductConsignment> _productsInStock;

        public Shop(string name, string address)
        {
            Id = _shopUniqueId++;
            Name = name ?? throw new ArgumentNullException(
                nameof(name),
                $"{nameof(name)} can't be null!");

            Address = address ?? throw new ArgumentNullException(
                nameof(address),
                $"{nameof(address)} can't be null!");

            _productsInStock = new List<ProductConsignment>();
        }

        public int Id { get; }
        public string Name { get; }
        public string Address { get; }
        public IReadOnlyCollection<ProductConsignment> Assortment => _productsInStock;

        public void AddNewProducts(List<ProductConsignment> productsToAdd, bool restockIfExists = true)
        {
            foreach (ProductConsignment productConsignment in productsToAdd)
            {
                ProductConsignment existingProduct = FindProductConsignment(productConsignment.Product);
                if (existingProduct == null)
                {
                    if (!restockIfExists)
                        throw new ShopException("Product already exists in shop and restockIfExists = false!");

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

        public uint CountOfProducts(Product product)
        {
            ProductConsignment consignment = FindProductConsignment(product);
            return consignment?.Count ?? 0u;
        }

        public decimal? PriceOnProducts(BuyingRequest request)
        {
            uint count = CountOfProducts(request.Product);
            if (count == 0)
                return null;
            return FindProductConsignment(request.Product).Price * count;
        }

        public void SetPrice(Product product, decimal price)
        {
            if (price <= 0)
                throw new ShopException("You can't set price <= 0!");
            ProductConsignment productConsignment = FindProductConsignment(product);
            if (productConsignment == null)
                throw new ShopException($"Product {product.Name} can't be found in shop {Name}");

            productConsignment.Price = price;
        }

        public void Buy(Person person, params BuyingRequest[] shoppingList)
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
            for (int positionNumber = 0; positionNumber < shoppingList.Length; positionNumber++)
                productsToBuy[positionNumber].Count -= shoppingList[positionNumber].Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_productsInStock, Id, Name, Address);
        }

        public override bool Equals(object obj)
        {
            return obj is Shop shop && Equals(shop);
        }

        public bool Equals(Shop other)
        {
            return other != null
                   && _productsInStock.All(other._productsInStock.Contains)
                   && _productsInStock.Count == other._productsInStock.Count
                   && Id == other.Id
                   && Name == other.Name
                   && Address == other.Address;
        }
    }
}