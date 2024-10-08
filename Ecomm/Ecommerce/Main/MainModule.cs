using Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Main
{
    internal class MainModule
    {
        private readonly EComService _orderService;

        public MainModule(EComService orderService)
        {
            _orderService = orderService;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---Welcome to the E-commerce System---");
                Console.ResetColor();
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Choose an option: ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _orderService.RegisterCustomer();
                        break;
                    case "2":
                        string userRole = _orderService.Login();
                        if (userRole == "Admin")
                        {
                            AdminMenu(); 
                        }
                        else if (userRole == "User")
                        {
                            LoggedInMenu(); 
                        }
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please choose again.");
                        Console.ResetColor();
                        break;
                }
            }
        }


        public void LoggedInMenu()
        {
            while (true)
            {
                Console.WriteLine("\nUser Menu:");
                Console.WriteLine("1. Add to Cart");
                Console.WriteLine("2. View Cart");
                Console.WriteLine("3. Place Order");
                Console.WriteLine("4. Return to Home Page");
                Console.WriteLine("5. Exit");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Choose an option: ");
                Console.ResetColor(); 
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _orderService.AddToCart();
                        break;
                    case "2":
                        _orderService.ViewCart();
                        break;
                    case "3":
                        _orderService.PlaceOrder();
                        break;
                    
                    case "4":
                        ShowMenu();
                        return;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please choose again.");
                        Console.ResetColor();
                        break;
                }
            }

        }
        public void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\nAdmin Menu:");
                Console.WriteLine("1. Create Product");
                Console.WriteLine("2. Delete Product");
                Console.WriteLine("3. View Customer Orders");
                Console.WriteLine("4. Return to Home Page");
                Console.WriteLine("5. Exit");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Choose an option: ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _orderService.CreateProduct();
                        break;
                    case "2":
                        _orderService.DeleteProduct();
                        break;
                    case "3":
                        _orderService.ViewCustomerOrder();
                        break;
                    case "4":
                        ShowMenu();
                        return;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please choose again.");
                        Console.ResetColor();
                        break;
                }
            }

        }

    }
}




          

