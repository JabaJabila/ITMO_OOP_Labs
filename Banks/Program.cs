using Banks.UI;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var ui = new ConsoleUI(new BanksViewModel());
            ui.Run();
        }
    }
}