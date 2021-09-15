using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Spectre.Console;

namespace Shops.UI
{
    public class Menu
    {
        private readonly ConsoleUI _ui;

        internal Menu(ConsoleUI ui)
        {
            _ui = ui ?? throw new UiException("UI object can't be null");
        }

        internal bool MainMenuDialogue(ShopManager manager)
        {
            string command = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose [green]command[/]...")
                    .PageSize(10)
                    .MoreChoicesText("[italic grey](Move up and down to reveal more commands)[/]")
                    .AddChoices(new[]
                    {
                        "1. Create shop",
                        "2. Register product",
                        "3. View all shops",
                        "4. View all products",
                        "5. Choose shop",
                        "6. Find cheapest shop",
                        "7. Exit",
                    }));

            switch (command)
            {
                case "1. Create shop":
                    _ui.CreatingShopDialogue();
                    return true;
                case "2. Register product":
                    _ui.RegisterProduct();
                    return true;
                case "3. View all shops":
                    TableGenerator.Generate(manager.AllShops);
                    return true;
                case "4. View all products":
                    TableGenerator.Generate(manager.AllProducts);
                    return true;
                case "5. Choose shop":
                    _ui.ChoosingShopDialogue();
                    return true;
                case "6. Find cheapest shop":
                    _ui.FindingCheapestShopDialogue();
                    return true;
                case "7. Exit":
                    return !AnsiConsole.Confirm("[bold]Are you sure you want to [red]exit?[/][/]\n");
                default:
                    throw new UiException("Unknown command error!");
            }
        }

        internal bool ShopMenuDialogue(Shop shop)
        {
            string command = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose [green]command[/]...")
                    .PageSize(10)
                    .MoreChoicesText("[italic grey](Move up and down to reveal more commands)[/]")
                    .AddChoices(new[]
                    {
                        "1. Add products",
                        "2. Set prices",
                        "3. Buy",
                        "4. View assortment",
                        "5. Back to main menu",
                    }));

            switch (command)
            {
                case "1. Add products":
                    _ui.AddingProductsToShopDialogue(shop);
                    return true;
                case "2. Set prices":
                    _ui.SettingPricesDialogue(shop);
                    return true;
                case "3. Buy":
                    _ui.BuyingDialogue(shop);
                    return true;
                case "4. View assortment":
                    TableGenerator.Generate(shop.Assortment);
                    return true;
                case "5. Back to main menu":
                    return false;
                default:
                    throw new UiException("Unknown command error!");
            }
        }
    }
}