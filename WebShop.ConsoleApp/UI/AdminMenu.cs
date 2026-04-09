using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.ConsoleApp.UI
{
    public class AdminMenu
    {
        // kla
        private readonly ICustomerRepository _ICustomerRepository;
        private readonly IProductRepository _productRepository;


        public AdminMenu(ICustomerRepository ICustomerRepository, IProductRepository productRepository)
        {
            _ICustomerRepository = ICustomerRepository;
            _productRepository = productRepository;
        }


        public async Task ShowMenuAsync()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("Admin Menu");
                Console.WriteLine("1.Manage Products");
                Console.WriteLine("2.Manage Customers");
                Console.WriteLine("3.Exit");
                Console.Write("\nSelect an option:");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ManageProductsAsync();
                        break;

                    case "2":
                        await ManageCustomersAsync();
                        break;

                    case "3":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        private async Task ManageCustomersAsync()
        {
            Console.Clear();
            Console.WriteLine("--Manage Customers--");
            Console.WriteLine("1. Edit Customers Profile");
            Console.WriteLine("2. View Order History");
            Console.WriteLine("\nselect an option");

            var choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.WriteLine("\n[ edit customer Profile ]");
            }
            else if (choice == "2")
            {
                Console.WriteLine("\n[ view order history ]");
            }

            Console.WriteLine("\nPress Enter to return");
            Console.ReadLine();
        }


        private async Task ManageProductsAsync()
        {
            Console.Clear();
            Console.WriteLine("--Manage Porducts--");
            Console.WriteLine("1.Add New Product");
            Console.WriteLine("2. Edit product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("\nSelect an Option");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                await AddProductAsync();
            }
            else
            {
                Console.WriteLine("\nFeature under construction");
            }

            Console.WriteLine("\nPress Enter to return");
            Console.ReadLine();
        }
        private async Task AddProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--Add New Product--");

            Console.Write("Enter product name: ");
            var name = Console.ReadLine();

            Console.Write("Enter product description: ");
            var description = Console.ReadLine();

            Console.Write("Enter product price: ");
            var priceInput = Console.ReadLine();

            // om användaren inte skriver in en giltig decimal, så visar vi ett felmeddelande och återvänder till menyn.
            if (!decimal.TryParse(priceInput, out decimal price))
            {
                Console.WriteLine("Invalid price. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            var newProduct = new Product
            {
                Name = name?? "Unknown",
                Description = description?? "Unknown",
                Price = price
            };

            // Vi använder repositoryt för att lägga till den nya produkten i databasen.
            await _productRepository.AddProductAsync(newProduct);

            Console.WriteLine("\nProduct added successfully! Press Enter to return.");
            return;
        }

      
        
        
        
        
        
        
        
    }
}
        
        
        
        
        