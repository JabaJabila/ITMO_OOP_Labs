using System.Collections.Generic;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Spectre.Console;

namespace Shops.UI
{
    public class Menu
    {
        private static readonly List<string> MainMenuCommandList = new (new[]
        {
            "1. Create shop",
            "2. Register product",
            "3. View all shops",
            "4. View all products",
            "5. Choose shop",
            "6. Find cheapest shop",
            "7. Exit",
        });

        private static readonly List<string> ShopMenuCommandList = new (new[]
        {
            "1. Add products",
            "2. Set prices",
            "3. Buy",
            "4. View assortment",
            "5. Back to main menu",
        });

        private readonly ConsoleUI _ui;

        internal Menu(ConsoleUI ui)
        {
            _ui = ui ?? throw new UIException("UI object can't be null");
        }

        internal bool MainMenuDialogue(ShopManager manager)
        {
            string command = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose [green]command[/]...")
                    .MoreChoicesText("[italic grey](Move up and down to reveal more commands)[/]")
                    .AddChoices(MainMenuCommandList));

            switch (MainMenuCommandList.IndexOf(command))
            {
                case 0:
                    _ui.CreatingShopDialogue();
                    return true;
                case 1:
                    _ui.RegisterProduct();
                    return true;
                case 2:
                    TableGenerator.Generate(manager.AllShops);
                    return true;
                case 3:
                    TableGenerator.Generate(manager.AllProducts);
                    return true;
                case 4:
                    _ui.ChoosingShopDialogue();
                    return true;
                case 5:
                    _ui.FindingCheapestShopDialogue();
                    return true;
                case 6:
                    return !AnsiConsole.Confirm("[bold]Are you sure you want to [red]exit?[/][/]\n");
                default:
                    throw new UIException("Unknown command error!");
            }
        }

        internal bool ShopMenuDialogue(Shop shop)
        {
            string command = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose [green]command[/]...")
                    .MoreChoicesText("[italic grey](Move up and down to reveal more commands)[/]")
                    .AddChoices(ShopMenuCommandList));

            switch (ShopMenuCommandList.IndexOf(command))
            {
                case 0:
                    _ui.AddingProductsToShopDialogue(shop);
                    return true;
                case 1:
                    _ui.SettingPricesDialogue(shop);
                    return true;
                case 2:
                    _ui.BuyingDialogue(shop);
                    return true;
                case 3:
                    TableGenerator.Generate(shop.Assortment);
                    return true;
                case 4:
                    return false;
                default:
                    throw new UIException("Unknown command error!");
            }
        }
    }
}