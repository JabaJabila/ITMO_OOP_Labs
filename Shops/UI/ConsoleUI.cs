using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Spectre.Console;

namespace Shops.UI
{
    public class ConsoleUI
    {
        private readonly ShopManager _manager;
        private readonly Menu _menus;

        public ConsoleUI(ShopManager shopManager)
        {
            _manager = shopManager ?? throw new UiException("Shop Manager can't be null!");
            _menus = new Menu(this);
        }

        public void Run()
        {
            bool inMainMenu = true;
            while (inMainMenu)
            {
                inMainMenu = _menus.MainMenuDialogue(_manager);
            }
        }

        internal void CreatingShopDialogue()
        {
            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [green]name[/] of the shop...")
                    .DefaultValue("NewShop"));
            string address = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [green]address[/] of the shop...")
                    .DefaultValue("SomeAddress"));
            try
            {
                _manager.CreateShop(name, address);
            }
            catch (ShopException exception)
            {
                AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
            }
        }

        internal void RegisterProduct()
        {
            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [green]name[/] of the product...")
                    .DefaultValue("NewProduct"));
            try
            {
                _manager.RegisterProduct(name);
            }
            catch (ProductException exception)
            {
                AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
            }
        }

        internal void ChoosingShopDialogue()
        {
            int shopId = ChoosingShopDialogue(
                _manager.AllShops.Select(shop => Convert.ToString(shop.Id) + ": " + shop.Name).ToList());
            try
            {
                Shop shop = _manager.GetShop(shopId);
                bool inShopMenu = true;
                while (inShopMenu)
                {
                    inShopMenu = _menus.ShopMenuDialogue(shop);
                }
            }
            catch (Exception exception)
            {
                AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
            }
        }

        internal void SettingPricesDialogue(Shop shop)
        {
            List<int> productIds = MultiChoosingProductsDialogue(
                shop.Assortment.Select(
                consignment => Convert.ToString(
                    consignment.Product.Id) + ": " + consignment.Product.Name).ToList());
            foreach (int id in productIds)
            {
                try
                {
                    Product product = _manager.GetProduct(id);
                    decimal price = AnsiConsole.Ask<decimal>(
                        $"What's [green]new price[/] of [yellow]{product.Name}[/]?");
                    shop.SetPrice(product, price);
                }
                catch (Exception exception)
                {
                    AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
                }
            }
        }

        internal void AddingProductsToShopDialogue(Shop shop)
        {
            var productConsignment = new List<ProductConsignment>();
            List<int> productIds = MultiChoosingProductsDialogue(
                _manager.AllProducts.Select(
                    product => Convert.ToString(product.Id) + ": " + product.Name).ToList());
            foreach (Product product in productIds.Select(id => _manager.GetProduct(id)))
            {
                try
                {
                    uint productsCount = AnsiConsole.Ask<uint>(
                        $"[green]How many[/] of [yellow]{product.Name}[/] you want to add?");
                    decimal price = AnsiConsole.Ask<decimal>(
                        $"What's [green]price[/] of [yellow]{product.Name}[/]?");
                    productConsignment.Add(new ProductConsignment(product, productsCount, price));
                }
                catch (Exception exception)
                {
                    AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
                }
            }

            shop.AddProducts(productConsignment);
        }

        internal void BuyingDialogue(Shop shop)
        {
            List<BuyingRequest> shoppingList = MakeShoppingList(
                MultiChoosingProductsDialogue(
                _manager.AllProducts.Select(
                    product => Convert.ToString(product.Id) + ": " + product.Name).ToList()));

            Person person = AddingPersonDialogue();
            if (person == null || shoppingList.Count == 0)
                return;
            try
            {
                shop.Buy(person, shoppingList);
                AnsiConsole.Render(new Rule("[bold green]Successfully bought[/]"));
            }
            catch (Exception exception)
            {
                AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
            }
        }

        internal void FindingCheapestShopDialogue()
        {
            List<BuyingRequest> shoppingList = MakeShoppingList(
                MultiChoosingProductsDialogue(
                _manager.AllProducts.Select(
                    product => Convert.ToString(product.Id) + ": " + product.Name).ToList()));
            Shop shop = _manager.FindCheapestShop(shoppingList);
            if (shop == null)
            {
                AnsiConsole.Render(new Rule("[bold yellow]Cheapest shop not found[/]"));
                return;
            }

            AnsiConsole.Render(new Rule($"[green]Cheapest shop found | ID: " +
                                        $"{shop.Id} | Name: {shop.Name} | Address: {shop.Address}[/]"));
            bool inShopMenu = true;
            while (inShopMenu)
            {
                inShopMenu = _menus.ShopMenuDialogue(shop);
            }
        }

        private List<BuyingRequest> MakeShoppingList(List<int> productIds)
        {
            var shoppingList = new List<BuyingRequest>();
            foreach (Product product in productIds.Select(id => _manager.GetProduct(id)))
            {
                try
                {
                    uint productsCount = AnsiConsole.Ask<uint>(
                        $"[green]How many[/] of [yellow]{product.Name}[/] you want to add?");
                    shoppingList.Add(new BuyingRequest(product, productsCount));
                }
                catch (Exception exception)
                {
                    AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
                }
            }

            return shoppingList;
        }

        private Person AddingPersonDialogue()
        {
            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [green]name[/] of the person...")
                    .DefaultValue("SomeName"));
            decimal balance = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Input [green]balance[/] of the person...")
                    .DefaultValue(0M));
            try
            {
                var person = new Person(name, balance);
                return person;
            }
            catch (PersonException exception)
            {
                AnsiConsole.Render(new Rule($"[bold red]{exception.Message}[/]"));
            }

            return null;
        }

        private int ChoosingShopDialogue(List<string> options)
        {
            string shopInfo = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose [green]products[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more products)[/]")
                    .AddChoices(options));
            return Convert.ToInt32(shopInfo.Split(':')[0]);
        }

        private List<int> MultiChoosingProductsDialogue(List<string> options)
        {
            List<string> productsInfo = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Choose [green]products[/]:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more products)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a product, "
                        + "[green]<enter>[/] to accept)[/]")
                    .AddChoices(options));
            return productsInfo.Select(product => Convert.ToInt32(product.Split(':')[0])).ToList();
        }
    }
}