using System;
using System.Collections.Generic;
using System.Globalization;
using Shops.Entities;
using Spectre.Console;

namespace Shops.UI
{
    internal static class TableGenerator
    {
        internal static void Generate(IEnumerable<Shop> shopList)
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Address");
            foreach (Shop shop in shopList)
                table.AddRow(Convert.ToString(shop.Id), shop.Name, shop.Address);
            AnsiConsole.Render(table);
        }

        internal static void Generate(IEnumerable<Product> productList)
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            foreach (Product product in productList)
                table.AddRow(Convert.ToString(product.Id), product.Name);
            AnsiConsole.Render(table);
        }

        internal static void Generate(IEnumerable<ProductConsignment> assortment)
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Count");
            table.AddColumn("Price");
            foreach (ProductConsignment consignment in assortment)
            {
                table.AddRow(
                    Convert.ToString(consignment.Product.Id),
                    consignment.Product.Name,
                    Convert.ToString(consignment.Count),
                    Convert.ToString(consignment.Price, CultureInfo.CurrentCulture));
            }

            AnsiConsole.Render(table);
        }
    }
}