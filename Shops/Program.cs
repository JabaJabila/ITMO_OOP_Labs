using Shops.Models;
using Shops.Services;
using Shops.UI;

namespace Shops
{
    internal class Program
    {
        private static void Main()
        {
            new ConsoleUI(new ShopLogicViewModel(new ShopManager())).Run();
        }
    }
}
