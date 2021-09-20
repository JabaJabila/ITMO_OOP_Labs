using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;

namespace Shops.Models
{
    public class ShopLogicViewModel
    {
        private readonly ShopManager _manager;

        public ShopLogicViewModel(ShopManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(
                nameof(manager),
                $"{nameof(manager)} can't be null!");
        }

        public IEnumerable<string> GetDataForShopTable(char tableSeparator)
        {
            return _manager.AllShops
                .Select(shop =>
                    Convert.ToString(shop.Id) + tableSeparator +
                    shop.Name + tableSeparator +
                    shop.Address);
        }

        public IEnumerable<string> GetDataForProductTable(char tableSeparator)
        {
            return _manager.AllProducts
                .Select(product =>
                    Convert.ToString(product.Id) + tableSeparator +
                    product.Name);
        }

        public IEnumerable<string> GetDataForAssortmentTable(char tableSeparator,  int shopId)
        {
            return _manager.GetShop(shopId).Assortment
                .Select(productConsignment =>
                    Convert.ToString(productConsignment.Product.Id) + tableSeparator +
                    productConsignment.Product.Name + tableSeparator +
                    Convert.ToString(productConsignment.Count) + tableSeparator +
                    Convert.ToString(productConsignment.Price, CultureInfo.CurrentCulture));
        }

        public string GetProductName(int productId)
        {
            return _manager.GetProduct(productId).Name;
        }

        public string GetShopName(int shopId)
        {
            return _manager.GetShop(shopId).Name;
        }

        public string GetShopAddress(int shopId)
        {
            return _manager.GetShop(shopId).Address;
        }

        public void CreateAndRegisterShop(string shopName, string shopAddress)
        {
            _manager.RegisterShop(new Shop(shopName, shopAddress));
        }

        public void CreateAndRegisterProduct(string productName)
        {
            _manager.RegisterProduct(new Product(productName));
        }

        public bool CheckIfShopsCreated()
        {
            return _manager.AllShops.Count != 0;
        }

        public string[] GetShopList()
        {
            return _manager.AllShops
                .Select(shop => Convert.ToString(shop.Id) + ": " + shop.Name)
                .ToArray();
        }

        public string[] GetAssortmentList(int shopId)
        {
            return _manager
                .GetShop(shopId).Assortment
                .Select(consignment => Convert.ToString(consignment.Product.Id) + ": " + consignment.Product.Name)
                .ToArray();
        }

        public string[] GetAllProductsList()
        {
            return _manager.AllProducts
                .Select(product => Convert.ToString(product.Id) + ": " + product.Name)
                .ToArray();
        }

        public void SetPrice(int shopId, int productId, decimal price)
        {
            _manager.GetShop(shopId).SetPrice(_manager.GetProduct(productId), price);
        }

        public bool CheckIfAssortmentEmpty(int shopId)
        {
            return _manager.GetShop(shopId).Assortment.Count == 0;
        }

        public bool CheckIfProductsRegistered()
        {
            return _manager.AllProducts.Count != 0;
        }

        public void AddProductConsignmentToShop(int shopId, int productId, uint count, decimal price)
        {
            _manager.GetShop(shopId).AddNewProducts(new List<ProductConsignment>
            {
                new (_manager.GetProduct(productId), count, price),
            });
        }

        public void Buy(
            int shopId,
            string personName,
            decimal personBalance,
            List<int> productIds,
            List<uint> productCount)
        {
            Shop shop = _manager.GetShop(shopId);
            var person = new Person(personName, personBalance);
            shop.Buy(person, MakeShoppingList(productIds, productCount));
        }

        public int FindCheapestShop(int productId, uint count)
        {
            return _manager.FindCheapestShop(new BuyingRequest(_manager.GetProduct(productId), count)).Id;
        }

        private BuyingRequest[] MakeShoppingList(IReadOnlyCollection<int> productIds, IReadOnlyList<uint> productCount)
        {
            if (productIds.Count != productCount.Count)
                throw new ModelException("Impossible to create ShoppingList!");

            return productIds
                .Select((id, counter) => new BuyingRequest(_manager.GetProduct(id), productCount[counter]))
                .ToArray();
        }
    }
}