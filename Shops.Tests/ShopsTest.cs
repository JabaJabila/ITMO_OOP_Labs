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
            _manager.CreateShop("Pyatyorochka", "Kupchinskaya st. 23.1A");
            Shop shop1 = _manager.CreateShop("Perekrestok", "Buharestkaya st. 89");
            Shop shop2 = _manager.CreateShop("Lenta", "Buharestkaya st. 69");
            Shop shop3 = _manager.CreateShop("Magnit", "Oleko Dundicha st. 36");
            Product buckwheat = _manager.RegisterProduct("Grechka 1kg");
            Product monster = _manager.RegisterProduct("Monster Black energy drink"); 
            Product bread = _manager.RegisterProduct("Hleb Borodinskiy");

            var consignmentList1 = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 5, 65M),
                new ProductConsignment(bread, 10, 40M),
                new ProductConsignment(monster, 3, 110M),
            };
            
            var consignmentList2 = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 4, 60M),
                new ProductConsignment(bread, 15, 44M),
                new ProductConsignment(monster, 12, 120M),
            };
            
            var consignmentList3 = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 6, 70M),
                new ProductConsignment(bread, 15, 36M),
                new ProductConsignment(monster, 20, 116M),
            };
            
            shop1.AddProducts(consignmentList1);
            shop2.AddProducts(consignmentList2);
            shop3.AddProducts(consignmentList3);
        }

        [TestCase("Buckwheat 1kg", 10u, 100)]
        [TestCase("CocaCola SugarFree 1l", 100u, 95)]
        [TestCase("Lays Onion flavor", 5u, 70)]
        public void AddProductsToShop_ShopHasProductsToBuy(string productName, uint count, decimal price)
        {
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct(productName);
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, count, price),
            };
            shop.AddProducts(consignmentList);
            Assert.AreEqual(product, shop.FindProductConsignment(product).Product);
            Assert.AreEqual(count, shop.FindProductConsignment(product).Count);
            Assert.AreEqual(price, shop.FindProductConsignment(product).Price);
        }
        
        [TestCase(10u, 100, 90)]
        [TestCase(100u, 95, 105)]
        [TestCase(5u, 70, 30)]
        public void SetPriceOnProduct_ProductChangedPrice(uint count, decimal oldPrice, decimal newPrice)
        {
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct("ProductName");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, count, oldPrice),
            };
            shop.AddProducts(consignmentList);
            Assert.AreEqual(oldPrice, shop.FindProductConsignment(product).Price);
            shop.SetPrice(product, newPrice);
            Assert.AreEqual(newPrice, shop.FindProductConsignment(product).Price);
        }
        
        [TestCase(10u, 1u, 100)]
        [TestCase(1u, 1u, 50)]
        [TestCase(20u, 20u, 10)]
        public void PersonBuysProducts_ProductsRemovesFromShop(uint countBefore, uint countToBuy, decimal price)
        {
            uint countAfter = countBefore - countToBuy;
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct("ProductName");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, countBefore, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("PersonName", price * countToBuy);
            shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(product, countToBuy)});

            Assert.AreEqual(countAfter, shop.FindProductConsignment(product).Count);
        }
        
        [TestCase(100, 10, 10u)]
        [TestCase(10000, 1, 95u)]
        [TestCase(200, 45, 3u)]
        public void PersonBuysProducts_PersonSpendMoney(decimal moneyBefore, decimal price, uint countToBuy)
        {
            decimal moneyAfter = moneyBefore - price * countToBuy;
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct("ProductName");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, countToBuy, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("PersonName", moneyBefore);
            shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(product, countToBuy)});

            Assert.AreEqual(moneyAfter, buyer.Balance);
        }
        
        [TestCase(10u, 11u, 10)]
        [TestCase(1u, 2u, 50)]
        [TestCase(20u, 200u, 30)]
        public void PersonBuysTooMuchProducts_ThrowException(uint countInShop, uint countToBuy, decimal price)
        {
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct("ProductName");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, countInShop, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("PersonName", price * countToBuy);
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(product, countToBuy)});
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
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product productInShop = _manager.RegisterProduct(productNameInShop);
            Product productToBuy = _manager.RegisterProduct(productNameToBuy);
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(productInShop, count, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("PersonName", price * count);
            
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(productToBuy, count)});
            });
        }
        
        [TestCase(100, 105, 1u)]
        [TestCase(1000, 100, 11u)]
        [TestCase(80, 9, 9u)]
        public void PersonDoesNotHaveEnoughMoneyToBuyProducts_ThrowException(decimal personMoney, decimal price, uint count)
        {
            Shop shop = _manager.CreateShop("ShopName", "ShopAddress");
            Product product = _manager.RegisterProduct("ProductName");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(product, count, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("PersonName", personMoney);

            Assert.Throws<PersonException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(product, count)});
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
            Product product = _manager.RegisterProduct("ProductName");
            Shop cheapShop = _manager.CreateShop("CheapShop", "ShopAddress");
            Shop expensiveShop = _manager.CreateShop("ExpensiveShop", "ShopAddress");
            var productConsignment1 = new List<ProductConsignment>
            {
                new ProductConsignment(product, countToBuy, priceInCheapShop)
            };
            var productConsignment2 = new List<ProductConsignment>
            {
                new ProductConsignment(product, countToBuy, priceInExpensiveShop)
            };
            var shoppingList = new List<BuyingRequest>()
            {
                new BuyingRequest(product, countToBuy)
            };
            cheapShop.AddProducts(productConsignment1);
            expensiveShop.AddProducts(productConsignment2);
            Assert.AreEqual(_manager.FindCheapestShop(shoppingList), cheapShop);
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
            Product product = _manager.RegisterProduct("ProductName");
            Shop cheapShop = _manager.CreateShop("CheapShop", "ShopAddress");
            Shop expensiveShop = _manager.CreateShop("ExpensiveShop", "ShopAddress");
            var productConsignment1 = new List<ProductConsignment>
            {
                new ProductConsignment(product, countInShop, priceInCheapShop)
            };
            var productConsignment2 = new List<ProductConsignment>
            {
                new ProductConsignment(product, countInShop, priceInExpensiveShop)
            };
            var shoppingList = new List<BuyingRequest>()
            {
                new BuyingRequest(product, countToBuy)
            };
            cheapShop.AddProducts(productConsignment1);
            expensiveShop.AddProducts(productConsignment2);
            Assert.Null(_manager.FindCheapestShop(shoppingList));
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
            Product exisingProduct = _manager.RegisterProduct(nameOfExistingProduct);
            Product nonExistingProduct = _manager.RegisterProduct(nameOfNonExistingProduct);
            Shop cheapShop = _manager.CreateShop("CheapShop", "ShopAddress");
            Shop expensiveShop = _manager.CreateShop("ExpensiveShop", "ShopAddress");
            var productConsignment1 = new List<ProductConsignment>
            {
                new ProductConsignment(exisingProduct, countToBuy, priceInCheapShop)
            };
            var productConsignment2 = new List<ProductConsignment>
            {
                new ProductConsignment(exisingProduct, countToBuy, priceInExpensiveShop)
            };
            var shoppingList = new List<BuyingRequest>()
            {
                new BuyingRequest(nonExistingProduct, countToBuy)
            };
            cheapShop.AddProducts(productConsignment1);
            expensiveShop.AddProducts(productConsignment2);
            Assert.Null(_manager.FindCheapestShop(shoppingList));
        }
    }
}