using System.Collections.Generic;

namespace Shops.UI
{
    public interface IConsoleUI
    {
        string AskForName(string target);
        string AskShopAddress();
        public decimal AskPersonBalance();
        public decimal AskForPrice(string name);
        public uint AskForProductCount(string name);
        public void ShowException(string message);
        public void ShowSuccessfulTransaction();
        public void ShowWarning(string message);
        void ShowSuccessfullyFoundShop(int id, string name, string address);
        int SingleSelection(string target, params string[] options);
        List<int> MultiSelectionProducts(params string[] options);
        bool ExitConfirmation();
        void GenerateTable(char separator, params string[] rows);
    }
}