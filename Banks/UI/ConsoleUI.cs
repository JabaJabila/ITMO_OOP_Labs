using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Exceptions;
using Spectre.Console;

namespace Banks.UI
{
    public class ConsoleUI
    {
        private static readonly string[] LoadProgressMenuCommands =
        {
            "1: Start new Bank System",
            "2: Load Bank System",
            "3: Exit",
        };

        private static readonly string[] CentralBankMenuCommands =
        {
            "1: Select bank",
            "2: Transactions history",
            "3: Add new bank",
            "4: Skip days",
            "5: Exit",
        };

        private static readonly string[] BankMenuCommands =
        {
            "1: AddClient",
            "2: Create new account",
            "3: Change account config",
            "4: Select account",
            "5: Select client",
            "6: Exit",
        };

        private static readonly string[] AccountMenuCommands =
        {
            "1: Start put transaction",
            "2: Start withdraw transaction",
            "3: Start transfer transaction",
            "4: Exit",
        };

        private static readonly string[] TransactionHistoryMenuCommands =
        {
            "1: Cancel transaction",
            "2: Exit",
        };

        private static readonly string[] ClientMenuCommands =
        {
            "1: Add passport",
            "2: Add address",
            "3: Subscribe on account changes",
            "4: Exit",
        };

        private static readonly string[] AccountTypesMenuCommands =
        {
            "1: Credit account",
            "2: Debit account",
            "3: Deposit account",
        };

        private readonly BanksViewModel _model;

        public ConsoleUI(BanksViewModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void Run()
        {
            bool inMainMenu = true;
            while (inMainMenu)
            {
                inMainMenu = LoadProgressMenuDialogue();
            }
        }

        private static string AskForName(string target)
            => AnsiConsole.Ask<string>($"What's [green]name[/] of the {target}?");

        private static string AskForString(string target)
            => AnsiConsole.Ask<string>($"What's [green]{target}[/]?");

        private static void ShowException(string message)
            => AnsiConsole.Write(new Rule($"[bold red]{message}[/]"));

        private static decimal AskForMoney(string target)
            => AnsiConsole.Ask<decimal>($"What's [green]{target}[/]$?");

        private static double AskForInterestRate(string name)
            => AnsiConsole.Ask<double>($"How big [green]interest rate[/] for {name} is?");

        private static int AskForCount(string target)
            => AnsiConsole.Ask<int>($"How many [green]{target}[/]?");

        private static void ShowSuccessfulTransaction()
            => AnsiConsole.Write(new Rule("[bold green]Transaction succeed[/]"));

        private static void ShowWarning(string message)
            => AnsiConsole.Write(new Rule($"[bold yellow]{message}[/]"));

        private static int SingleSelection(string target, params string[] options)
        {
            string info = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Choose [green]{target}[/]:")
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(options));
            return Convert.ToInt32(info.Split(':')[0]);
        }

        private static bool ExitConfirmation()
            => AnsiConsole.Confirm("[bold]Are you sure you want to [red]exit?[/][/]\n");

        private bool LoadProgressMenuDialogue()
        {
            int command = SingleSelection("command", LoadProgressMenuCommands);
            switch (command)
            {
                case 1: CentralBankCreationDialogue(); break;
                case 2: CentralBankSelectionDialogue(); break;
                case 3: return !ExitConfirmation();
                default: throw new UIException("Unknown command chosen!");
            }

            return true;
        }

        private void CentralBankSelectionDialogue()
        {
            string[] options = _model.GetCentralBanksInfo().ToArray();
            if (options.Length == 0)
            {
                ShowWarning("No central banks were created!");
                return;
            }

            int id = SingleSelection("central bank", options);
            _model.FindCentralBankById(id);
            RunCentralBankMenuDialogue();
        }

        private void CentralBankCreationDialogue()
        {
            _model.StartNewCentralBank();
            RunCentralBankMenuDialogue();
        }

        private void RunCentralBankMenuDialogue()
        {
            bool inCentralBankMenu = true;
            while (inCentralBankMenu)
            {
                inCentralBankMenu = CentralBankMenuDialogue();
            }
        }

        private bool CentralBankMenuDialogue()
        {
            int command = SingleSelection("command", CentralBankMenuCommands);
            switch (command)
            {
                case 1: BankSelectionDialogue(); break;
                case 2: TransactionHistoryDialogue(); break;
                case 3: AddingNewBankDialogue(); break;
                case 4: SkippingDaysDialogue(); break;
                case 5: return false;
                default: throw new UIException("Unknown command chosen!");
            }

            return true;
        }

        private void SkippingDaysDialogue()
        {
            int days = AskForCount("days");

            try
            {
                _model.SkipDays(days);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void AddingNewBankDialogue()
        {
            string name = AskForName("bank");
            double debitInterest = AskForInterestRate("debit interest rate");
            decimal untrustedLimit = AskForMoney("untrusted limit");
            decimal minusLimit = AskForMoney("minus limit");
            decimal commission = AskForMoney("commission");
            double depositInterest = AskForInterestRate("deposit default interest rate");
            int count = AskForCount("Deposit interest stages");
            var borders = new List<decimal>();
            var rates = new List<double>();
            for (int counter = 0; counter < count; counter++)
            {
                borders.Add(AskForMoney("amount for interest rate"));
                rates.Add(AskForInterestRate("this stage"));
            }

            try
            {
                RunBankMenuDialogue(_model.AddNewBank(
                        name,
                        untrustedLimit,
                        debitInterest,
                        commission,
                        minusLimit,
                        depositInterest,
                        borders,
                        rates));
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void TransactionHistoryDialogue()
        {
            int command = SingleSelection("command", TransactionHistoryMenuCommands);
            switch (command)
            {
                case 1:
                    string[] options = _model.GetTransactionsInfo().ToArray();
                    if (options.Length == 0)
                    {
                        ShowWarning("No transactions...");
                        return;
                    }

                    int transactionId = SingleSelection(
                        "transaction",
                        options);
                    _model.CancelTransaction(transactionId);
                    break;
                case 2: return;
                default: throw new UIException("Unknown command chosen!");
            }
        }

        private void BankSelectionDialogue()
        {
            string[] options = _model.GetBanksInfo().ToArray();
            if (options.Length == 0)
            {
                ShowWarning("No banks were created!");
                return;
            }

            int bankId = SingleSelection("bank", options);
            RunBankMenuDialogue(bankId);
        }

        private void RunBankMenuDialogue(int bankId)
        {
            bool inBankMenu = true;
            while (inBankMenu)
            {
                inBankMenu = BankMenuDialogue(bankId);
            }
        }

        private bool BankMenuDialogue(int bankId)
        {
            int command = SingleSelection("command", BankMenuCommands);
            switch (command)
            {
                case 1: AddClientDialogue(bankId); break;
                case 2: CreateAccountDialogue(bankId); break;
                case 3: ChangeAccountConfigDialogue(bankId); break;
                case 4:
                    string[] accounts = _model.GetAccountsOfBankInfo(bankId).ToArray();
                    if (accounts.Length == 0)
                    {
                        ShowWarning("No accounts were created!");
                        break;
                    }

                    AccountMenuDialogue(SingleSelection("account", accounts));
                    break;

                case 5:
                    string[] clients = _model.GetClientsInfo(bankId).ToArray();
                    if (clients.Length == 0)
                    {
                        ShowWarning("No clients found in bank!");
                        break;
                    }

                    ClientMenuDialogue(SingleSelection("client", clients), bankId);
                    break;
                case 6: return false;
                default: throw new UIException("Unknown command chosen!");
            }

            return true;
        }

        private void ClientMenuDialogue(int clientId, int bankId)
        {
            string info;
            try
            {
                info = _model.GetClientInfo(clientId);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
                return;
            }

            AnsiConsole.Write(new Markup(info));
            int command = SingleSelection("command", ClientMenuCommands);
            switch (command)
            {
                case 1:
                    string passport = AskForString("passport");
                    try
                    {
                        _model.ChangePassport(clientId, passport);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 2:
                    string address = AskForString("address");
                    try
                    {
                        _model.ChangeAddress(clientId, address);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 3:
                    int typeId = SingleSelection("account type", AccountTypesMenuCommands);
                    try
                    {
                        _model.SubscribeOnChanges(clientId, bankId, typeId);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 4:
                    return;
                default: throw new UIException("Unknown command chosen!");
            }
        }

        private void AccountMenuDialogue(int accountId)
        {
            string info;
            try
            {
                info = _model.GetAccountInfo(accountId);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
                return;
            }

            AnsiConsole.Write(new Markup(info));
            int command = SingleSelection("command", AccountMenuCommands);
            switch (command)
            {
                case 1:
                    decimal amountToPut = AskForMoney("money to put");
                    try
                    {
                        _model.PutMoney(accountId, amountToPut);
                        ShowSuccessfulTransaction();
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 2:
                    decimal amountToWithdraw = AskForMoney("money to withdraw");
                    try
                    {
                        _model.WithdrawMoney(accountId, amountToWithdraw);
                        ShowSuccessfulTransaction();
                    }
                    catch (Exception exception)
                    {
                         ShowException(exception.Message);
                    }

                    break;

                case 3:
                    decimal amountToTransfer = AskForMoney("money to transfer");
                    int accToId = SingleSelection(
                        "account type",
                        _model.GetAllAccountsInfo().ToArray());
                    try
                    {
                        _model.TransferMoney(accountId, accToId, amountToTransfer);
                        ShowSuccessfulTransaction();
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 4:
                    return;
                default: throw new UIException("Unknown command chosen!");
            }
        }

        private void CreateAccountDialogue(int bankId)
        {
            string[] clients = _model.GetClientsInfo(bankId).ToArray();
            if (clients.Length == 0)
            {
                ShowWarning("No clients found in bank!");
                return;
            }

            int clientId = SingleSelection("client", clients);
            int command = SingleSelection("type of account", AccountTypesMenuCommands);
            decimal balance = AskForMoney("balance");
            switch (command)
            {
                case 1:
                    try
                    {
                        _model.CreateCreditAccount(bankId, clientId, balance);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 2:
                    try
                    {
                        _model.CreateDebitAccount(bankId, clientId, balance);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;

                case 3:
                    try
                    {
                        int daysToSave = AskForCount("days you want to save money");
                        _model.CreateDepositAccount(bankId, clientId, balance, daysToSave);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception.Message);
                    }

                    break;
            }
        }

        private void AddClientDialogue(int bankId)
        {
            string name = AskForName("client");
            string surname = AskForString("surname");
            try
            {
                ClientMenuDialogue(_model.CreateClient(bankId, name, surname), bankId);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }

        private void ChangeAccountConfigDialogue(int bankId)
        {
            double debitInterest = AskForInterestRate("debit interest rate");
            decimal untrustedLimit = AskForMoney("untrusted limit");
            decimal minusLimit = AskForMoney("minus limit");
            decimal commission = AskForMoney("commission");
            double depositInterest = AskForInterestRate("deposit default interest rate");
            int count = AskForCount("Deposit interest stages");
            var borders = new List<decimal>();
            var rates = new List<double>();
            for (int counter = 0; counter < count; counter++)
            {
                borders.Add(AskForMoney("amount for interest rate"));
                rates.Add(AskForInterestRate("this stage"));
            }

            try
            {
                _model.ChangeBankConfigs(
                    bankId,
                    untrustedLimit,
                    debitInterest,
                    commission,
                    minusLimit,
                    depositInterest,
                    borders,
                    rates);
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
            }
        }
    }
}