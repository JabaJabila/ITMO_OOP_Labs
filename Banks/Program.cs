using Banks.UI;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            using var vm = new BanksViewModel();
            var ui = new ConsoleUI(vm);
            ui.Run();
        }
    }
}