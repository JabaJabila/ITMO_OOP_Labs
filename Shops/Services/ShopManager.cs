using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopManager
    {
        private const int DefaultShopStarterId = 1;
        private const int DefaultProductStarterId = 1;
        private readonly List<Shop> _allShops;
        private readonly List<Product> _allProducts;
        private int _shopUniqueId;
        private int _productUniqueId;

        public ShopManager(int productStarterId = DefaultProductStarterId, int shopStarterId = DefaultShopStarterId)
        {
            _productUniqueId = productStarterId;
            _shopUniqueId = shopStarterId;
            _allProducts = new List<Product>();
            _allShops = new List<Shop>();
        }

        public IReadOnlyList<Product> AllProducts => _allProducts;
        public IReadOnlyList<Shop> AllShops => _allShops;

        public Shop CreateShop(string name, string address)
        {
            var newShop = new Shop(_shopUniqueId++, name, address);
            _allShops.Add(newShop);
            return newShop;
        }

        public Product RegisterProduct(string name)
        {
            var newProduct = new Product(_productUniqueId++, name);
            _allProducts.Add(newProduct);
            return newProduct;
        }

        public Shop FindShop(string name)
        {
            return _allShops.FirstOrDefault(shop => shop.Name == name);
        }

        public Shop GetShop(int id)
        {
            foreach (Shop shop in _allShops.Where(shop => shop.Id == id))
                return shop;
            throw new ShopException($"Shop with id={id} doesn't exists!");
        }

        public Product FindProduct(string name)
        {
            return _allProducts.FirstOrDefault(product => product.Name == name);
        }

        public Product GetProduct(int id)
        {
            foreach (Product product in _allProducts.Where(product => product.Id == id))
                return product;
            throw new ProductException($"Product with id={id} doesn't exists!");
        }

        public Shop FindCheapestShop(List<BuyingRequest> shoppingList)
        {
            decimal minimalPrice = 0M;
            Shop profitableShop = null;
            foreach (Shop shop in _allShops)
            {
                decimal totalPrice = 0M;
                bool validShop = true;
                foreach (BuyingRequest buyingRequest in shoppingList)
                {
                    ProductConsignment currentConsignment = shop.FindProductConsignment(buyingRequest.Product);
                    if (currentConsignment == null || currentConsignment.Count < buyingRequest.Count)
                    {
                        validShop = false;
                        break;
                    }

                    totalPrice += currentConsignment.Price * buyingRequest.Count;
                }

                if (!validShop || (profitableShop != null && totalPrice >= minimalPrice)) continue;
                minimalPrice = totalPrice;
                profitableShop = shop;
            }

            return profitableShop;
        }
    }
}