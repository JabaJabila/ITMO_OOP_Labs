using System.Collections.Generic;
using Shops.Services;
using Shops.Entities;
using Shops.Tools;
using NUnit.Framework;

namespace Shops.Tests
{
    [TestFixture]
    public class Tests
    {
        private ShopManager _manager;
        [SetUp]
        public void Setup()
        {
            _manager = new ShopManager();
        }

        [TestCase("Buckwheat 1kg", 10u, 100)]
        [TestCase("CocaCola SugarFree 1l", 100u, 95)]
        [TestCase("Lays Onion flavor", 5u, 70)]
        public void AddProductsToShop_ShopHasProductsToBuy(string productName,
            uint count, 
            decimal price)
        {
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new(productName);
            shop.AddNewProducts(new List<ProductConsignment> {new (product, count, price)});
            Assert.AreEqual(product, shop.FindProductConsignment(product).Product);
            Assert.AreEqual(count, shop.FindProductConsignment(product).Count);
            Assert.AreEqual(price, shop.FindProductConsignment(product).Price);
        }
        
        [TestCase(10u, 100, 90)]
        [TestCase(100u, 95, 105)]
        [TestCase(5u, 70, 30)]
        public void SetPriceOnProduct_ProductChangedPrice(
            uint count,
            decimal oldPrice,
            decimal newPrice)
        {
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new("ProductName");
            shop.AddNewProducts(new List<ProductConsignment> {new (product, count, oldPrice)});
            Assert.AreEqual(oldPrice, shop.FindProductConsignment(product).Price);
            shop.SetPrice(product, newPrice);
            Assert.AreEqual(newPrice, shop.FindProductConsignment(product).Price);
        }
        
        [TestCase(10u, 1u, 100)]
        [TestCase(1u, 1u, 50)]
        [TestCase(20u, 20u, 10)]
        public void PersonBuysProducts_ProductsRemovesFromShop(
            uint countBefore, 
            uint countToBuy, 
            decimal price)
        {
            uint countAfter = countBefore - countToBuy;
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new("ProductName");
            shop.AddNewProducts(new List<ProductConsignment> {new (product, countBefore, price)});
            var buyer = new Person("PersonName", price * countToBuy);
            shop.Buy(buyer, new BuyingRequest(product, countToBuy));

            Assert.AreEqual(countAfter, shop.FindProductConsignment(product).Count);
        }
        
        [TestCase(100, 10, 10u)]
        [TestCase(10000, 1, 95u)]
        [TestCase(200, 45, 3u)]
        public void PersonBuysProducts_PersonSpendMoney(
            decimal moneyBefore, 
            decimal price, 
            uint countToBuy)
        {
            decimal moneyAfter = moneyBefore - price * countToBuy;
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new("ProductName");
            shop.AddNewProducts(new List<ProductConsignment> {new (product, countToBuy, price)});
            var buyer = new Person("PersonName", moneyBefore);
            shop.Buy(buyer, new BuyingRequest(product, countToBuy));

            Assert.AreEqual(moneyAfter, buyer.Balance);
        }
        
        [TestCase(10u, 11u, 10)]
        [TestCase(1u, 2u, 50)]
        [TestCase(20u, 200u, 30)]
        public void PersonBuysTooMuchProducts_ThrowException(
            uint countInShop, 
            uint countToBuy, 
            decimal price)
        {
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new("ProductName");
            shop.AddNewProducts(new List<ProductConsignment>{new (product, countInShop, price)});
            var buyer = new Person("PersonName", price * countToBuy);
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new BuyingRequest(product, countToBuy));
            });
        }
        
        [TestCase("Buckwheat", "Rice", 5u, 100)]
        [TestCase("RedBull", "MonsterBlack", 1u, 120)]
        [TestCase("Bread", "Buns", 3u, 50)]
        public void PersonBuysNonExistentProduct_ThrowException(
            string productNameToBuy, 
            string productNameInShop, 
            uint count, 
            decimal price)
        {
            Shop shop = new("ShopName", "ShopAddress");
            Product productInShop = new(productNameInShop);
            Product productToBuy = new(productNameToBuy);
            shop.AddNewProducts(new List<ProductConsignment> {new (productInShop, count, price)});
            var buyer = new Person("PersonName", price * count);
            
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new BuyingRequest(productToBuy, count));
            });
        }
        
        [TestCase(100, 105, 1u)]
        [TestCase(1000, 100, 11u)]
        [TestCase(80, 9, 9u)]
        public void PersonDoesNotHaveEnoughMoneyToBuyProducts_ThrowException(
            decimal personMoney,
            decimal price,
            uint count)
        {
            Shop shop = new("ShopName", "ShopAddress");
            Product product = new("ProductName");
            shop.AddNewProducts(new List<ProductConsignment> { new(product, count, price)});
            var buyer = new Person("PersonName", personMoney);

            Assert.Throws<PersonException>(() =>
            {
                shop.Buy(buyer, new BuyingRequest(product, count));
            });
        }
        
        [TestCase(10, 20, 5u)]
        [TestCase(100, 1001, 1u)]
        [TestCase(15, 150, 3u)]
        public void SearchForMinimalPrice_SuccessfullyFound(
            decimal priceInCheapShop,
            decimal priceInExpensiveShop,
            uint countToBuy)
        {
            Product product = new("ProductName");
            Shop cheapShop = new("CheapShop", "ShopAddress");
            Shop expensiveShop = new("ExpensiveShop", "ShopAddress");
            _manager.RegisterProduct(product);
            _manager.RegisterShop(cheapShop);
            _manager.RegisterShop(expensiveShop);
 
            cheapShop.AddNewProducts(new List<ProductConsignment> 
                {new (product, countToBuy, priceInCheapShop)});
            expensiveShop.AddNewProducts(new List<ProductConsignment> 
                {new (product, countToBuy, priceInExpensiveShop)});
            Assert.AreEqual(_manager.FindCheapestShop(new BuyingRequest(product, countToBuy)), cheapShop);
        }
        
        [TestCase(10, 20, 5u, 3u)]
        [TestCase(100, 1001, 2u, 1u)]
        [TestCase(15, 150, 30u, 3u)]
        public void SearchForMinimalPriceButNotEnoughProducts_NotFoundReturnsNull(
            decimal priceInCheapShop,
            decimal priceInExpensiveShop,
            uint countToBuy,
            uint countInShop)
        {
            Product product = new("ProductName");
            Shop cheapShop = new("CheapShop", "ShopAddress");
            Shop expensiveShop = new("ExpensiveShop", "ShopAddress");
            _manager.RegisterProduct(product);
            _manager.RegisterShop(cheapShop);
            _manager.RegisterShop(expensiveShop);

            cheapShop.AddNewProducts(new List<ProductConsignment>
                {new (product, countInShop, priceInCheapShop)});
            expensiveShop.AddNewProducts(new List<ProductConsignment>
                {new (product, countInShop, priceInExpensiveShop)});
            Assert.Null(_manager.FindCheapestShop(new BuyingRequest(product, countToBuy)));
        }
        
        [TestCase(10, 20, 5u, "CocaCola", "Fanta")]
        [TestCase(100, 1001, 2u, "Onion", "Garlic")]
        [TestCase(15, 150, 30u, "Green Tea", "Black Tea")]
        public void SearchForMinimalPriceForNonExistingProduct_NotFoundReturnsNull(
            decimal priceInCheapShop,
            decimal priceInExpensiveShop,
            uint countToBuy,
            string nameOfExistingProduct,
            string nameOfNonExistingProduct)
        {
            Product exisingProduct = new(nameOfExistingProduct);
            Product nonExistingProduct = new(nameOfNonExistingProduct);
            Shop cheapShop = new("CheapShop", "ShopAddress");
            Shop expensiveShop = new("ExpensiveShop", "ShopAddress");
            _manager.RegisterProduct(exisingProduct);
            _manager.RegisterProduct(nonExistingProduct);
            _manager.RegisterShop(cheapShop);
            _manager.RegisterShop(expensiveShop);
 
            cheapShop.AddNewProducts(new List<ProductConsignment> 
                {new (exisingProduct, countToBuy, priceInCheapShop)});
            expensiveShop.AddNewProducts(new List<ProductConsignment> 
                {new (exisingProduct, countToBuy, priceInExpensiveShop)});
            Assert.Null(_manager.FindCheapestShop(new BuyingRequest(nonExistingProduct, countToBuy)));
        }
    }
}