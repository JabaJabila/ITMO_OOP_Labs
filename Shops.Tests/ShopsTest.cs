using System.Collections.Generic;
using Shops.Services;
using Shops.PersonEntities;
using Shops.ShopEntities;
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

        [Test]
        public void AddProductsToShop_ShopHasProductsToBuy()
        {
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            Product bread = _manager.FindProduct("Hleb Borodinskiy");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 5, 95M),
                new ProductConsignment(bread, 10, 45M),
            };
            shop.AddProducts(consignmentList);
            Assert.AreEqual(bread, shop.FindProductConsignment(bread).Product);
            Assert.AreEqual(buckwheat, shop.FindProductConsignment(buckwheat).Product);
        }
        
        [Test]
        public void SetPriceOnProduct_ProductChangedPrice()
        {
            const decimal priceBefore = 100M;
            const decimal priceAfter = 110M;
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 10, priceBefore),
            };
            shop.AddProducts(consignmentList);
            shop.SetPrice(buckwheat, priceAfter);
            
            Assert.AreEqual(priceAfter, shop.FindProductConsignment(buckwheat).Price);
        }
        
        [Test]
        public void PersonBuysProducts_ProductsRemovesFromShop()
        {
            const uint countBefore = 10;
            const uint countToBuy = 2;
            const uint countAfter = countBefore - countToBuy;
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, countBefore, 100M),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("Vasya", 1000M);
            shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(buckwheat, countToBuy)});

            Assert.AreEqual(countAfter, shop.FindProductConsignment(buckwheat).Count);
        }
        
        [Test]
        public void PersonBuysProducts_PersonSpendMoney()
        {
            const decimal moneyBefore = 1000M;
            const decimal price = 105M;
            const decimal moneyAfter = moneyBefore - price;
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 10, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("Vasya", moneyBefore);
            shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(buckwheat, 1)});

            Assert.AreEqual(moneyAfter, buyer.Balance);
        }
        
        [Test]
        public void PersonBuysTooMuchProducts_ThrowException()
        {
            const uint countBefore = 10;
            const uint countToBuy = 11;
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, countBefore, 100M),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("Vasya", 1200M);
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(buckwheat, countToBuy)});
            });
        }
        
        [Test]
        public void PersonBuysNonExistentProduct_ThrowException()
        {
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            Product monster = _manager.FindProduct("Monster Black energy drink");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, 10, 100M),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("Vasya", 1200M);
            Assert.Throws<ShopException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(monster, 1)});
            });
        }
        
        [Test]
        public void PersonDoesNotHaveEnoughMoneyToBuyProducts_ThrowException()
        {
            const decimal price = 105M;
            const uint productCount = 10;
            const decimal money = price * (productCount - 1);
            Shop shop = _manager.FindShop("Pyatyorochka");
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var consignmentList = new List<ProductConsignment>
            {
                new ProductConsignment(buckwheat, productCount, price),
            };
            shop.AddProducts(consignmentList);
            var buyer = new Person("Vasya", money);

            Assert.Throws<PersonException>(() =>
            {
                shop.Buy(buyer, new List<BuyingRequest>{new BuyingRequest(buckwheat, productCount)});
            });
        }
        
        [Test]
        public void SearchForMinimalPrice_SuccessfullyFound()
        {
            Product buckwheat = _manager.FindProduct("Grechka 1kg");
            var shoppingList = new List<BuyingRequest> {new BuyingRequest(buckwheat, 1)};
            
            Assert.AreEqual(_manager.FindMostProfitableShop(shoppingList), _manager.FindShop("Lenta"));
        }
        
        [Test]
        public void SearchForMinimalPriceButNoEnoughProducts_NotFoundReturnsNull()
        {
            Product monster = _manager.FindProduct("Monster Black energy drink");
            var shoppingList = new List<BuyingRequest> {new BuyingRequest(monster, 30)};
            
            Assert.Null(_manager.FindMostProfitableShop(shoppingList));
        }
        
        [Test]
        public void SearchForMinimalPriceForNonExistingProduct_NotFoundReturnsNull()
        {
            Product juice = _manager.RegisterProduct("Orange juice Dobriy");
            var shoppingList = new List<BuyingRequest> {new BuyingRequest(juice, 1)};
            
            Assert.Null(_manager.FindMostProfitableShop(shoppingList));
        }
    }
}