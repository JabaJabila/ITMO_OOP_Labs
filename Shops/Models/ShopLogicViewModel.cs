using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Shops.UI;

namespace Shops.Models
{
    public class ShopLogicViewModel
    {
        private const char DefaultTableSeparator = '|';
        private static readonly string[] MainMenuCommands =
        {
            "1: Create shop",
            "2: Register product",
            "3: View all shops",
            "4: View all products",
            "5: Choose shop",
            "6: Find cheapest shop",
            "7: Exit",
        };

        private static readonly string[] ShopMenuCommands =
        {
            "1: Add products",
            "2: Set prices",
            "3: Buy",
            "4: View assortment",
            "5: Back to main menu",
        };

        private readonly char _tableSeparator;
        private readonly IConsoleUI _ui;
        private readonly ShopManager _manager;

        public ShopLogicViewModel(IConsoleUI ui, ShopManager manager, char tableSeparator = DefaultTableSeparator)
        {
            _ui = ui ?? throw new ModelException("UI can't be null!");
            _manager = manager ?? throw new ModelException("Manager can't be null!");
            _tableSeparator = tableSeparator;
        }

        public void Run()
        {
            bool inMainMenu = true;
            while (inMainMenu)
            {
                inMainMenu = MainMenuDialogue();
            }
        }

        private void CreatingShopDialogue()
        {
            string name = _ui.AskForName("shop");
            string address = _ui.AskShopAddress();
            try
            {
                _manager.RegisterShop(new Shop(name, address));
            }
            catch (ShopException exception)
            {
                _ui.ShowException(exception.Message);
            }
        }

        private void RegisterProductDialogue()
        {
            string name = _ui.AskForName("product");
            try
            {
                _manager.RegisterProduct(new Product(name));
            }
            catch (ProductException exception)
            {
                _ui.ShowException(exception.Message);
            }
        }

        private void ChoosingShopDialogue()
        {
            if (_manager.AllShops.Count == 0)
            {
                _ui.ShowWarning("No shops created!");
                return;
            }

            int shopId = _ui.SingleSelection("shop", _manager.AllShops
                .Select(shop => Convert.ToString(shop.Id) + ": " + shop.Name)
                .ToArray());
            try
            {
                Shop shop = _manager.GetShop(shopId);
                bool inShopMenu = true;
                while (inShopMenu)
                {
                    inShopMenu = ShopMenuDialogue(shop);
                }
            }
            catch (Exception exception)
            {
                _ui.ShowException(exception.Message);
            }
        }

        private void SettingPricesDialogue(Shop shop)
        {
            if (shop.Assortment.Count == 0)
            {
                _ui.ShowWarning("No products in shop!");
                return;
            }

            List<int> productIds = _ui.MultiSelectionProducts(shop.Assortment
                .Select(consignment => Convert.ToString(consignment.Product.Id) + ": " + consignment.Product.Name)
                .ToArray());

            foreach (int id in productIds)
            {
                try
                {
                    Product product = _manager.GetProduct(id);
                    decimal price = _ui.AskForPrice(product.Name);
                    shop.SetPrice(product, price);
                }
                catch (Exception exception)
                {
                    _ui.ShowException(exception.Message);
                }
            }
        }

        private void AddingProductsToShopDialogue(Shop shop)
        {
            if (_manager.AllProducts.Count == 0)
            {
                _ui.ShowWarning("No products created!");
                return;
            }

            var productConsignment = new List<ProductConsignment>();
            List<int> productIds = _ui.MultiSelectionProducts(
                _manager.AllProducts.Select(
                    product => Convert.ToString(product.Id) + ": " + product.Name).ToArray());
            foreach (Product product in productIds.Select(id => _manager.GetProduct(id)))
            {
                try
                {
                    uint productsCount = _ui.AskForProductCount(product.Name);
                    decimal price = _ui.AskForPrice(product.Name);
                    productConsignment.Add(new ProductConsignment(product, productsCount, price));
                }
                catch (Exception exception)
                {
                    _ui.ShowException(exception.Message);
                }
            }

            shop.AddNewProducts(productConsignment);
        }

        private void BuyingDialogue(Shop shop)
        {
            if (shop.Assortment.Count == 0)
            {
                _ui.ShowWarning("No products in shop!");
                return;
            }

            List<BuyingRequest> shoppingList = MakeShoppingList(
                _ui.MultiSelectionProducts(
                _manager.AllProducts
                    .Select(product => Convert.ToString(product.Id) + ": " + product.Name)
                    .ToArray()));

            Person person = AddingPersonDialogue();
            if (person == null || shoppingList.Count == 0)
                return;
            try
            {
                shop.Buy(person, shoppingList.ToArray());
                _ui.ShowSuccessfulTransaction();
            }
            catch (Exception exception)
            {
                _ui.ShowException(exception.Message);
            }
        }

        private void FindingCheapestShopDialogue()
        {
            int productId = _ui.SingleSelection("product", _manager.AllProducts
                .Select(product => Convert.ToString(product.Id) + ": " + product.Name)
                .ToArray());

            try
            {
                Product product = _manager.GetProduct(productId);
                uint countToBuy = _ui.AskForProductCount(product.Name);
                BuyingRequest request = new (product, countToBuy);

                Shop shop = _manager.FindCheapestShop(request);
                if (shop == null)
                {
                    _ui.ShowWarning("Cheapest shop wasn't found...");
                    return;
                }

                _ui.ShowSuccessfullyFoundShop(shop.Id, shop.Name, shop.Address);
                bool inShopMenu = true;
                while (inShopMenu)
                {
                    inShopMenu = ShopMenuDialogue(shop);
                }
            }
            catch (Exception exception)
            {
                _ui.ShowException(exception.Message);
            }
        }

        private List<BuyingRequest> MakeShoppingList(List<int> productIds)
        {
            var shoppingList = new List<BuyingRequest>();
            foreach (Product product in productIds.Select(id => _manager.GetProduct(id)))
            {
                try
                {
                    uint productsCount = _ui.AskForProductCount(product.Name);
                    shoppingList.Add(new BuyingRequest(product, productsCount));
                }
                catch (Exception exception)
                {
                    _ui.ShowException(exception.Message);
                }
            }

            return shoppingList;
        }

        private Person AddingPersonDialogue()
        {
            string name = _ui.AskForName("person");
            decimal balance = _ui.AskPersonBalance();
            try
            {
                var person = new Person(name, balance);
                return person;
            }
            catch (PersonException exception)
            {
                _ui.ShowException(exception.Message);
            }

            return null;
        }

        private bool MainMenuDialogue()
        {
            int command = _ui.SingleSelection("command", MainMenuCommands);

            switch (command)
            {
                case 1: CreatingShopDialogue(); break;
                case 2: RegisterProductDialogue(); break;
                case 3:
                    var shopRows = new List<string>
                        { "ID" + _tableSeparator + "Name" + _tableSeparator + "Address" };
                    shopRows.AddRange(_manager.AllShops
                        .Select(shop =>
                            Convert.ToString(shop.Id) + _tableSeparator +
                            shop.Name + _tableSeparator +
                            shop.Address));

                    _ui.GenerateTable(_tableSeparator, shopRows.ToArray());
                    break;
                case 4:
                    var productRows = new List<string> { "ID" + _tableSeparator + "Name" };
                    productRows.AddRange(_manager.AllProducts
                        .Select(product =>
                            Convert.ToString(product.Id) + _tableSeparator +
                            product.Name));

                    _ui.GenerateTable(_tableSeparator, productRows.ToArray());
                    break;
                case 5: ChoosingShopDialogue(); break;
                case 6: FindingCheapestShopDialogue(); break;
                case 7: return !_ui.ExitConfirmation();
                default: throw new UIException("Unknown command error!");
            }

            return true;
        }

        private bool ShopMenuDialogue(Shop shop)
        {
            int command = _ui.SingleSelection("command", ShopMenuCommands);

            switch (command)
            {
                case 1: AddingProductsToShopDialogue(shop); break;
                case 2: SettingPricesDialogue(shop); break;
                case 3: BuyingDialogue(shop); break;
                case 4:
                    var assortmentRows = new List<string>
                        { "ID" + _tableSeparator + "Name" + _tableSeparator + "Count" + _tableSeparator + "Price" };
                    assortmentRows.AddRange(shop.Assortment
                        .Select(productConsignment =>
                            Convert.ToString(productConsignment.Product.Id) + _tableSeparator +
                            productConsignment.Product.Name + _tableSeparator +
                            Convert.ToString(productConsignment.Count) + _tableSeparator +
                            Convert.ToString(productConsignment.Price, CultureInfo.CurrentCulture)));

                    _ui.GenerateTable(_tableSeparator, assortmentRows.ToArray());
                    break;
                case 5: return false;
                default: throw new UIException("Unknown command error!");
            }

            return true;
        }
    }
}