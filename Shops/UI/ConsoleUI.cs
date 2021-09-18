using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Shops.UI
{
    public class ConsoleUI : IConsoleUI
    {
        public string AskForName(string target)
        {
            return AnsiConsole.Ask<string>($"What's [green]name[/] of the {target}?");
        }

        public string AskShopAddress()
        {
            return AnsiConsole.Ask<string>("What's [green]address[/] of the shop?");
        }

        public void ShowException(string message)
        {
            AnsiConsole.Render(new Rule($"[bold red]{message}[/]"));
        }

        public decimal AskForPrice(string name)
        {
            return AnsiConsole.Ask<decimal>($"What's [green]price[/] of [yellow]{name}[/]?");
        }

        public uint AskForProductCount(string name)
        {
            return AnsiConsole.Ask<uint>($"[green]How many[/] of [yellow]{name}[/] you want to add?");
        }

        public void ShowSuccessfulTransaction()
        {
            AnsiConsole.Render(new Rule("[bold green]Successfully bought[/]"));
        }

        public void ShowWarning(string message)
        {
            AnsiConsole.Render(new Rule($"[bold yellow]{message}[/]"));
        }

        public void ShowSuccessfullyFoundShop(int id, string name, string address)
        {
            AnsiConsole.Render(new Rule($"[green]Cheapest shop found | ID: " +
                                        $"{id} | Name: {name} | Address: {address}[/]"));
        }

        public string AskPersonName()
        {
            return AnsiConsole.Ask<string>("What's [green]name[/] of the person?");
        }

        public decimal AskPersonBalance()
        {
            return AnsiConsole.Ask<decimal>("What's [green]balance[/] of the person?");
        }

        public int SingleSelection(string target, params string[] options)
        {
            string shopInfo = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Choose [green]{target}[/]:")
                    .MoreChoicesText("[grey](Move up and down to reveal more products)[/]")
                    .AddChoices(options));
            return Convert.ToInt32(shopInfo.Split(':')[0]);
        }

        public List<int> MultiSelectionProducts(params string[] options)
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

        public bool ExitConfirmation()
        {
            return AnsiConsole.Confirm("[bold]Are you sure you want to [red]exit?[/][/]\n");
        }

        public void GenerateTable(char separator, params string[] rows)
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
    }
}