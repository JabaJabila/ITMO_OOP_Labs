using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopManager
    {
        private readonly List<Shop> _allShops;
        private readonly List<Product> _allProducts;

        public ShopManager()
        {
            _allProducts = new List<Product>();
            _allShops = new List<Shop>();
        }

        public IReadOnlyCollection<Product> AllProducts => _allProducts;
        public IReadOnlyCollection<Shop> AllShops => _allShops;

        public void RegisterShop(Shop shop)
        {
            if (shop == null)
                throw new ArgumentNullException(nameof(shop), $"{nameof(shop)} can't be null!");

            _allShops.Add(shop);
        }

        public void RegisterProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _allProducts.Add(product);
        }

        public Shop GetShop(int id)
        {
            return _allShops.FirstOrDefault(shop => shop.Id == id) ??
                   throw new ShopException($"Shop with id={id} doesn't exists!");
        }

        public Product GetProduct(int id)
        {
            return _allProducts.FirstOrDefault(product => product.Id == id) ??
                   throw new ProductException($"Product with id={id} doesn't exists!");
        }

        public Shop FindCheapestShop(BuyingRequest request)
        {
            return _allShops
                .Where(shop => shop.CountOfProducts(request.Product) >= request.Count)
                .OrderBy(shop => shop.PriceOnProducts(request))
                .FirstOrDefault();
        }
    }
}