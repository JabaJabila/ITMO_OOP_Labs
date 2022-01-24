using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Models;
using Spectre.Console;

namespace Shops.UI
{
    public class ConsoleUI
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
        private readonly ShopLogicViewModel _model;

        public ConsoleUI(ShopLogicViewModel model, char tableSeparator = DefaultTableSeparator)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

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

        private static string AskForName(string target)
            => AnsiConsole.Ask<string>($"What's [green]name[/] of the {target}?");

        private static string AskShopAddress()
            => AnsiConsole.Ask<string>("What's [green]address[/] of the shop?");

        private static void ShowException(string message)
            => AnsiConsole.Render(new Rule($"[bold red]{message}[/]"));

        private static decimal AskForPrice(string name)
            => AnsiConsole.Ask<decimal>($"What's [green]price[/] of [yellow]{name}[/]?");

        private static uint AskForProductCount(string name)
            => AnsiConsole.Ask<uint>($"[green]How many[/] of [yellow]{name}[/] you want to add?");

        private static void ShowSuccessfulTransaction()
            => AnsiConsole.Render(new Rule("[bold green]Successfully bought[/]"));

        private static void ShowWarning(string message)
            => AnsiConsole.Render(new Rule($"[bold yellow]{message}[/]"));

        private static void ShowSuccessfullyFoundShop(int id, string name, string address)
            => AnsiConsole.Render(new Rule($"[green]Cheapest shop found | ID: " +
                                           $"{id} | Name: {name} | Address: {address}[/]"));

        private static decimal AskPersonBalance()
            => AnsiConsole.Ask<decimal>("What's [green]balance[/] of the person?");

        private static int SingleSelection(string target, params string[] options)
        {
            string shopInfo = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Choose [green]{target}[/]:")
                    .MoreChoicesText("[grey](Move up and down to reveal more products)[/]")
                    .AddChoices(options));
            return Convert.ToInt32(shopInfo.Split(':')[0]);
        }

        private static List<int> MultiSelectionProducts(params string[] options)
        {
            List<string> productsInfo = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Choose [green]products[/]:")
                    .MoreChoicesText("[grey](Move up and down to reveal more products)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a product, "
                        + "[green]<enter>[/] to accept)[/]")
                    .AddChoices(options));
            return productsInfo.Select(product => Convert.ToInt32(product.Split(':')[0])).ToList();
        }

        private static bool ExitConfirmation()
            => AnsiConsole.Confirm("[bold]Are you sure you want to [red]exit?[/][/]\n");

        private static void GenerateTable(char separator, params string[] rows)
        {
            if (rows.Length == 0)
                throw new UIException("Table must have at least header!");

            var table = new Table();
            string[] header = rows[0].Split(separator);
            int columnCount = header.Length;

            foreach (string head in header)
                table.AddColumn(head);

            foreach (string row in rows[1..])
            {
                string[] elements = row.Split(separator);

                if (elements.Length != columnCount)
                    throw new UIException($"Wrong table format (columns: {columnCount}; rows: {elements.Length})!");

                table.AddRow(elements);
            }

            AnsiConsole.Render(table);
        }

        private void ShopMenu(int shopId)
        {
            bool inShopMenu = true;
            while (inShopMenu)
            {
                inShopMenu = ShopMenuDialogue(shopId);
            }
        }

        private bool MainMenuDialogue()
        {
            int command = SingleSelection("command", MainMenuCommands);

            switch (command)
            {
                case 1: CreateShopDialogue(); break;
                case 2: RegisterProductDialogue(); break;
                case 3:
                    var shopRows = new List<string>
                        { "ID" + _tableSeparator + "Name" + _tableSeparator + "Address" };
                    shopRows.AddRange(_model.GetDataForShopTable(_tableSeparator));
                    GenerateTable(_tableSeparator, shopRows.ToArray());
                    break;
                case 4:
                    var productRows = new List<string> { "ID" + _tableSeparator + "Name" };
                    productRows.AddRange(_model.GetDataForProductTable(_tableSeparator));
                    GenerateTable(_tableSeparator, productRows.ToArray());
                    break;
                case 5: ChoosingShopDialogue(); break;
                case 6: FindingCheapestShopDialogue(); break;
                case 7: return !ExitConfirmation();
                default: throw new UIException("Unknown command error!");
            }

            return true;
        }

        private bool ShopMenuDialogue(int shopId)
        {
            int command = SingleSelection("command", ShopMenuCommands);

            switch (command)
            {
                case 1: AddingProductsToShopDialogue(shopId); break;
                case 2: SettingPricesDialogue(shopId); break;
                case 3: BuyingDialogue(shopId); break;
                case 4:
                    var assortmentRows = new List<string>
                        { "ID" + _tableSeparator + "Name" + _tableSeparator + "Count" + _tableSeparator + "Price" };
                    assortmentRows.AddRange(_model.GetDataForAssortmentTable(_tableSeparator, shopId));
                    GenerateTable(_tableSeparator, assortmentRows.ToArray());
                    break;
                case 5: return false;
                default: throw new UIException("Unknown command error!");
            }

            return true;
        }

        private void CreateShopDialogue()
        {
            string shopName = AskForName("shop");
            string shopAddress = AskShopAddress();
            try
            {
                _model.CreateAndRegisterShop(shopName, shopAddress);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void RegisterProductDialogue()
        {
            string productName = AskForName("product");
            try
            {
                _model.CreateAndRegisterProduct(productName);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void ChoosingShopDialogue()
        {
            if (!_model.CheckIfShopsCreated())
            {
                ShowWarning("No shops created!");
                return;
            }

            try
            {
                int shopId = SingleSelection("shop", _model.GetShopList());
                ShopMenu(shopId);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void SettingPricesDialogue(int shopId)
        {
            if (_model.CheckIfAssortmentEmpty(shopId))
            {
                ShowWarning("No products in shop!");
                return;
            }

            List<int> productIds = MultiSelectionProducts(_model.GetAssortmentList(shopId));

            foreach (int productId in productIds)
            {
                try
                {
                    string productName = _model.GetProductName(productId);
                    decimal price = AskForPrice(productName);
                    if (price <= 0)
                        throw new UIException("Price can't be <= 0");

                    _model.SetPrice(shopId, productId, price);
                }
                catch (Exception exception)
                {
                    ShowException(exception.Message);
                }
            }
        }

        private void AddingProductsToShopDialogue(int shopId)
        {
            if (!_model.CheckIfProductsRegistered())
            {
                ShowWarning("No products created!");
                return;
            }

            List<int> productIds = MultiSelectionProducts(_model.GetAllProductsList());

            foreach (int productId in productIds)
            {
                try
                {
                    string productName = _model.GetProductName(productId);
                    uint productsCount = AskForProductCount(productName);
                    decimal price = AskForPrice(productName);
                    if (price <= 0)
                        throw new UIException("Price can't be <= 0");

                    _model.AddProductConsignmentToShop(shopId, productId, productsCount, price);
                }
                catch (Exception exception)
                {
                    ShowException(exception.Message);
                }
            }
        }

        private void BuyingDialogue(int shopId)
        {
            if (_model.CheckIfAssortmentEmpty(shopId))
            {
                ShowWarning("No products in shop!");
                return;
            }

            List<int> productIds = MultiSelectionProducts(_model.GetAssortmentList(shopId));
            try
            {
                var productCount = productIds
                    .Select(id => AskForProductCount(_model.GetProductName(id)))
                    .ToList();

                string personName = AskForName("person");
                decimal personBalance = AskPersonBalance();
                if (personBalance < 0)
                    throw new UIException("Person's balance can't be < 0");
                _model.Buy(shopId, personName, personBalance, productIds, productCount);
                ShowSuccessfulTransaction();
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void FindingCheapestShopDialogue()
        {
            if (!_model.CheckIfProductsRegistered())
            {
                ShowWarning("No products created!");
                return;
            }

            int productId = SingleSelection("product", _model.GetAllProductsList());

            try
            {
                string productName = _model.GetProductName(productId);
                uint countToBuy = AskForProductCount(productName);
                int shopId = _model.FindCheapestShop(productId, countToBuy);

                ShowSuccessfullyFoundShop(shopId, _model.GetShopName(shopId), _model.GetShopAddress(shopId));
                ShopMenu(shopId);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }
    }
}