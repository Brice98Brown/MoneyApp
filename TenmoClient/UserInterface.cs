using System;
using TenmoClient.Data;
using TenmoClient.APIClients;
using System.Collections.Generic;

namespace TenmoClient
{
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly AccountRestClient accountClient = new AccountRestClient();
        private readonly UserRestClient userClient = new UserRestClient();
        private readonly TransferRestClient transferClient = new TransferRestClient();

        private bool quitRequested = false;

        public void Start()
        {
            while (!quitRequested)
            {
                while (!authService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();
            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    AccountsModel accounts = accountClient.GetAccounts();

                    switch (menuSelection)
                    {
                        case 1: // View Balance
                            Console.WriteLine($"Your Current Account Balance is : {accounts.Balance.ToString("C")}");
                            break;

                        case 2: // View Past Transfers
                            List<TransferModel> transfers = transferClient.GetListOfTransfers(accounts.UserId);

                            Console.WriteLine("----------------------------------------------------------");
                            Console.WriteLine("Transfer ID".PadRight(20) + "From/To".PadRight(25) + "Amount");
                            Console.WriteLine("----------------------------------------------------------");

                            ListTransfers(accounts, transfers);
                            
                            Console.WriteLine("Please enter transfer ID to view details (0 to cancel):");
                           
                            GetTransferDetails(transfers);
                            
                            break;

                        case 3: // View Pending Requests
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 4: // Send TE Bucks

                            MakeATransfer(accounts);

                            break;

                        case 5: // Request TE Bucks
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 6: // Log in as someone else

                            authService.ClearAuthenticator();
                            Logout();
                            ShowLogInMenu();
                            // NOTE: You will need to clear any stored JWTs in other API Clients


                            return; // Leaves the menu and should return as someone else

                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            return;

                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        private static void ListTransfers(AccountsModel accounts, List<TransferModel> transfers)
        {
            foreach (TransferModel transfer in transfers)
            {
                if (transfer.RecipientUserId == accounts.UserId)
                {
                    //from
                    Console.WriteLine(transfer.TransferId.ToString().PadRight(20) + "From: " + transfer.SenderUsername.PadRight(21) + transfer.TransferAmount.ToString("C"));
                }
                else if (transfer.SenderUserId == accounts.UserId)
                {
                    //to
                    Console.WriteLine(transfer.TransferId.ToString().PadRight(20) + "To: " + transfer.RecipientUsername.PadRight(21) + transfer.TransferAmount.ToString("C"));
                }

            }
        }

        private void MakeATransfer(AccountsModel accounts)
        {
            DisplayAllUsers(accounts);//passing in the account of the person logged in so that it doesn't display
            Console.WriteLine("Enter ID of user you are sending to (0 to cancel):");
            int moneyRecipient; //= Console.ReadLine();
            if (!int.TryParse(Console.ReadLine(), out moneyRecipient))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (moneyRecipient == 0)
            {
                return;
            }
            else
            {
                List<UserModel> users = userClient.GetAllUsers();
                foreach (UserModel user in users)
                {
                    if (user.UserId == moneyRecipient)
                    {
                        GetTransferAmount(accounts, moneyRecipient);
                        return;
                    }
                }
                Console.WriteLine("Could not find Id. Maybe they went to go get milk?");
            }
        }

        private void GetTransferDetails(List<TransferModel> transfers)
        {
            if (!int.TryParse(Console.ReadLine(), out int transferIdInput))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (transferIdInput == 0)
            {
                return;
            }
            else
            {
                foreach (TransferModel transfer in transfers)
                {
                    if (transferIdInput == transfer.TransferId)
                    {
                        Console.WriteLine("----------------------------------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("----------------------------------------------------------");
                        Console.WriteLine("Id: " + transfer.TransferId);
                        Console.WriteLine("From: " + transfer.SenderUsername);
                        Console.WriteLine("To: " + transfer.RecipientUsername);
                        Console.WriteLine("Type: " + transfer.TransferType);
                        Console.WriteLine("Status: " + transfer.TransferStatus);
                        Console.WriteLine("Amount: " + transfer.TransferAmount.ToString("C"));
                        return;
                    }
                }
                Console.WriteLine("Invalid Id");
            }
        }
        private void GetTransferAmount(AccountsModel accounts, int moneyRecipient)
        {
            Console.WriteLine("Enter amount:");
            decimal transferAmount; //= Console.ReadLine();
            if (!decimal.TryParse(Console.ReadLine(), out transferAmount))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (transferAmount < 0)
            {
                Console.WriteLine("I'm positive that number wasn't.");
            }
            else if (transferAmount.ToString().Contains(".") && transferAmount.ToString().Substring(transferAmount.ToString().IndexOf(".")).Length > 3)
            {
                    Console.WriteLine("That amount doesn't make Cents, Bruh");
            }
            else
            {
                TransferModel transfer = new TransferModel();
                transfer.TransferAmount = transferAmount;
                transfer.RecipientUserId = moneyRecipient;
                transfer.SenderUserId = accounts.UserId;

                transferClient.NewTransfer(transfer);
                Console.WriteLine(transfer.TransferAmount.ToString("C") + " TE bucks transferred! Woot!");
            }
        }

        private void DisplayAllUsers(AccountsModel account)
        {
            List<UserModel> users = userClient.GetAllUsers();
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("User Id".PadRight(20) + "Username");
            Console.WriteLine("-----------------------------------------------");
            foreach (UserModel user in users)
            {
                if (user.UserId != account.UserId)
                    Console.WriteLine(user.UserId.ToString().PadRight(20) + user.Username);
            }
            Console.WriteLine("-----------------------------------------------");
        }

        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        private void HandleUserLogin()
        {
            while (!authService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();

                // Log the user in
                API_User authenticatedUser = authService.Login(loginUser);

                if (authenticatedUser != null)
                {
                    string jwt = authenticatedUser.Token;

                    // TODO: Do something with this JWT.
                    accountClient.UpdateToken(jwt);
                    userClient.GetToken(jwt);
                    transferClient.UpdateToken(jwt);

                }
            }
        }
        private void Logout()
        {
            accountClient.UpdateToken(null);
            userClient.GetToken(null);
            transferClient.UpdateToken(null);
        }
    }
}
