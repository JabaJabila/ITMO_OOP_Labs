using Shops.Services;
using Shops.UI;

namespace Shops
{
    internal class Program
    {
        private static void Main()
        {
            var consoleUi = new ConsoleUI(new ShopManager());
            consoleUi.Run();
        }
    }
}
