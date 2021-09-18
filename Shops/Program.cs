using Shops.Models;
using Shops.Services;
using Shops.UI;

namespace Shops
{
    internal class Program
    {
        private static void Main()
        {
            var application = new ShopLogicViewModel(new ConsoleUI(), new ShopManager());
            application.Run();
        }
    }
}
