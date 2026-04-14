using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _1.WebShop.Core.Enums;
using _4.WebShop.ConsoleApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // tillagd kc

namespace WebShop.ConsoleApp.UI
{
    public class AdminMenu
    {
        // Adminmenu som låter admin hantera produkter och kunder.

        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ConsoleNavigationService _navigation;

        // Konstruktor som tar in nödvändiga repository och navigation service.
        public AdminMenu(ICustomerRepository customerRepository, IProductRepository productRepository, ConsoleNavigationService navigation)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _navigation = navigation;
        }

        // Huvudmeny för admin där de kan välja att hantera produkter eller kunder.
        public async Task ShowMenuAsync()
        {
            var selector = new ListSelector<string>(_navigation);
            var options = new List<string> { "Manage Products", "Manage Customers", "Exit" };

            bool isRunning = true;
            while (isRunning)
            {
                var choice = selector.Select(options, item => item, "ADMIN MENU");

                if (choice == "Manage Products") await ManageProductsAsync();
                else if (choice == "Manage Customers") await ManageCustomersAsync();
                else if (choice == "Exit" || choice == null) isRunning = false;
            }
        }

        // Metod för att hantera produkter där admin kan välja att lägga till, redigera eller ta bort produkter.
        private async Task ManageProductsAsync()
        {
            var selector = new ListSelector<string>(_navigation);
            var options = new List<string> { "Add New Product", "Edit Product", "Delete Product", "Go Back" };

            var choice = selector.Select(options, item => item, "MANAGE PRODUCTS");

            if (choice == "Add New Product") await AddProductAsync();
            else if (choice == "Edit Product") await EditProductAsync();
            else if (choice == "Delete Product") await DeleteProductAsync();
        }

        // Metod för att lägga till en ny produkt där admin.
        private async Task AddProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Product ---");
            Console.WriteLine("(Type '0' at any time to cancel)\n");

            Console.Write("Enter product name: ");
            var name = Console.ReadLine();
            if (name == "0") return;

            Console.Write("Enter product category: ");
            var category = Console.ReadLine();
            if (category == "0") return;

            Console.Write("Enter product description: ");
            var description = Console.ReadLine();
            if (description == "0") return;

            Console.Write("Enter product supplier: ");
            var supplier = Console.ReadLine();
            if (supplier == "0") return;

            Size parsedSize;
            while (true)
            {
                Console.Write("Enter product size (Small, Medium, Large): ");
                string sizeInput = Console.ReadLine();
                if (sizeInput == "0") return;

                if (Enum.TryParse<Size>(sizeInput, true, out parsedSize)) break;
                Console.WriteLine("Error: Invalid size. Please check the spelling and try again.");
            }

            decimal price;
            while (true)
            {
                Console.Write("Enter product price: ");
                string priceInput = Console.ReadLine();
                if (priceInput == "0") return;

                if (decimal.TryParse(priceInput, out price) && price >= 0) break;
                Console.WriteLine("Error: Price must be a valid positive number.");
            }

            int inventory;
            while (true)
            {
                Console.Write("Enter inventory count: ");
                string inventoryInput = Console.ReadLine();
                if (inventoryInput == "0") return;

                if (int.TryParse(inventoryInput, out inventory) && inventory >= 0) break;
                Console.WriteLine("Error: Inventory must be a positive whole number.");
            }

            var newProduct = new Product
            {
                Name = string.IsNullOrWhiteSpace(name) ? "Unknown" : name,
                Category = string.IsNullOrWhiteSpace(category) ? "Unknown" : category,
                Description = string.IsNullOrWhiteSpace(description) ? "Unknown" : description,
                Supplier = string.IsNullOrWhiteSpace(supplier) ? "Unknown" : supplier,
                Size = parsedSize,
                Price = price,
                Inventory = inventory 
            };

            // Lägger till den nya produkten i repositoryt.
            await _productRepository.AddProductAsync(newProduct);
            Console.WriteLine("\nProduct added successfully! Press Enter to return.");
            Console.ReadLine();
        }

        // Metod för att redigera en befintlig produkt där admin kan söka efter produkter och uppdatera deras information.
        private async Task EditProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Edit Product ---");
            Console.WriteLine("(Type '0' at any time to cancel)\n");

            Console.Write("Search by product name or category (or press Enter to view all): ");
            string search = Console.ReadLine()?.Trim().ToLower();
            if (search == "0") return;

            var allProducts = await _productRepository.GetAllAsync();

            var matchingProducts = allProducts.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(search)) ||
                (p.Category != null && p.Category.ToLower().Contains(search))
            ).ToList();

            if (!matchingProducts.Any())
            {
                Console.WriteLine("\nNo matching products found. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nFound Products:");
            for (int i = 0; i < matchingProducts.Count; i++)
            {
                
                Console.WriteLine($"{i + 1}. {matchingProducts[i].Name} | {matchingProducts[i].Category} | {matchingProducts[i].Price} kr | Inventory: {matchingProducts[i].Inventory}");
            }

            Console.Write("\nEnter the number of the product to edit (or 0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > matchingProducts.Count)
            {
                Console.WriteLine("Invalid choice. Canceling...");
                Console.ReadLine();
                return;
            }

            // Om admin väljer 0 så avbryts redigeringen.
            if (choice == 0) return;
            var product = matchingProducts[choice - 1];

            Console.Clear();
            Console.WriteLine($"\nEditing Product: {product.Name}");
            Console.WriteLine("(Leave blank and press Enter to keep current value)");
            Console.WriteLine("---------------------------------------------------");

            Console.Write($"New name [{product.Name}]: ");
            var newName = Console.ReadLine();
            if (newName == "0") return;
            if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;

            while (true)
            {
                Console.Write($"New size [{product.Size}]: ");
                string sizeInput = Console.ReadLine();
                if (sizeInput == "0") return;
                if (string.IsNullOrWhiteSpace(sizeInput)) break;

                if (Enum.TryParse<Size>(sizeInput, true, out Size updatedSize))
                {
                    product.Size = updatedSize;
                    break;
                }
                Console.WriteLine("Error: Invalid size. Use Small, Medium, or Large.");
            }
            
            while (true)
            {
                Console.Write($"New price [{product.Price}]: ");
                string priceInput = Console.ReadLine();
                if (priceInput == "0") return;
                if (string.IsNullOrWhiteSpace(priceInput)) break;

                if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice >= 0)
                {
                    product.Price = newPrice;
                    break;
                }
                Console.WriteLine("Error: Please enter a valid positive number.");
            }
            
            while (true)
            {
                Console.Write($"New inventory [{product.Inventory}]: ");
                string inventoryInput = Console.ReadLine();
                if (inventoryInput == "0") return;
                if (string.IsNullOrWhiteSpace(inventoryInput)) break;

                if (int.TryParse(inventoryInput, out int newInventory) && newInventory >= 0)
                {
                    product.Inventory = newInventory;
                    break;
                }
                Console.WriteLine("Error: Inventory must be a positive whole number.");
            }

            await _productRepository.UpdateProductAsync(product);
            Console.WriteLine("\nProduct updated successfully! Press Enter to return.");
            Console.ReadLine();
        }

        // Metod för att ta bort en produkt där admin kan söka efter produkter och välja vilken de vill ta bort.
        private async Task DeleteProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Product ---");
            Console.WriteLine("(Type '0' at any time to cancel)\n");

            Console.Write("Search by product name or category to delete: ");
            string search = Console.ReadLine()?.Trim().ToLower();
            if (search == "0" || string.IsNullOrWhiteSpace(search)) return;

            var allProducts = await _productRepository.GetAllAsync();
            var matches = allProducts.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(search)) ||
                (p.Category != null && p.Category.ToLower().Contains(search))
            ).ToList();

            if (!matches.Any())
            {
                Console.WriteLine("\nNo matching products found. Press Enter.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nFound Products:");
            for (int i = 0; i < matches.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {matches[i].Name} | {matches[i].Category}");
            }

            Console.Write("\nSelect the number to DELETE (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= matches.Count)
            {
                var productToDelete = matches[choice - 1];
                await _productRepository.DeleteProductAsync(productToDelete.Id);
                Console.WriteLine($"\nProduct '{productToDelete.Name}' deleted successfully! Press Enter.");
            }
            else
            {
                Console.WriteLine("Invalid choice. Canceling...");
            }
            Console.ReadLine();
        }

        // Metod för att hantera kunder där admin kan välja att redigera kundprofiler eller visa orderhistorik.
        private async Task ManageCustomersAsync()
        {
            var selector = new ListSelector<string>(_navigation);
            var options = new List<string> { "Edit Customer Profile", "View Order History", "Go Back" };

            var choice = selector.Select(options, item => item, "MANAGE CUSTOMERS");

            if (choice == "Edit Customer Profile") await EditCustomerAsync();
            else if (choice == "View Order History") await ViewCustomerHistoryAsync();
        }

        // Metod för att redigera en kunds profil där admin kan uppdatera kundens information.
        private async Task EditCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Edit Customer Profile ---");
            Console.WriteLine("(Type '0' at any time to cancel)\n");

            Console.Write("Enter customer ID to edit: ");
            string idInput = Console.ReadLine();
            if (idInput == "0") return;

            if (int.TryParse(idInput, out int customerId))
            {
                var customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);

                if (customer == null)
                {
                    Console.WriteLine("Customer not found! Press Enter to return.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nEditing Customer ID {customer.Id} ({customer.Name})");
                Console.WriteLine("(Leave blank and press Enter to keep current value)");
                Console.WriteLine("---------------------------------------------------");

                Console.Write($"Name [{customer.Name}]: ");
                var newName = Console.ReadLine();
                if (newName == "0") return;
                if (!string.IsNullOrWhiteSpace(newName)) customer.Name = newName;

                Console.Write($"Email [{customer.Email}]: ");
                var newEmail = Console.ReadLine();
                if (newEmail == "0") return;
                if (!string.IsNullOrWhiteSpace(newEmail)) customer.Email = newEmail;

                Console.Write($"Address [{customer.Address}]: ");
                var newAddress = Console.ReadLine();
                if (newAddress == "0") return;
                if (!string.IsNullOrWhiteSpace(newAddress)) customer.Address = newAddress;

                Console.Write($"City [{customer.City}]: ");
                var newCity = Console.ReadLine();
                if (newCity == "0") return;
                if (!string.IsNullOrWhiteSpace(newCity)) customer.City = newCity;

                Console.Write($"Country [{customer.Country}]: ");
                var newCountry = Console.ReadLine();
                if (newCountry == "0") return;
                if (!string.IsNullOrWhiteSpace(newCountry)) customer.Country = newCountry;

                Console.Write($"Phone [{customer.phone}]: ");
                var newPhone = Console.ReadLine();
                if (newPhone == "0") return;
                if (!string.IsNullOrWhiteSpace(newPhone)) customer.phone = newPhone;

                while (true)
                {
                    Console.Write($"Age [{customer.Age}]: ");
                    var ageInput = Console.ReadLine();
                    if (ageInput == "0") return;
                    if (string.IsNullOrWhiteSpace(ageInput)) break;

                    if (int.TryParse(ageInput, out int newAge) && newAge > 0)
                    {
                        customer.Age = newAge;
                        break;
                    }
                    Console.WriteLine("Error: Age must be a positive number.");
                }

                Console.Write($"Gender [{customer.Gender}]: ");
                var newGender = Console.ReadLine();
                if (newGender == "0") return;
                if (!string.IsNullOrWhiteSpace(newGender)) customer.Gender = newGender;

                Console.Write($"Preferred Payment Method [{customer.PreferredPaymentMethod}]: ");
                var newPayment = Console.ReadLine();
                if (newPayment == "0") return;
                if (!string.IsNullOrWhiteSpace(newPayment)) customer.PreferredPaymentMethod = newPayment;

                await _customerRepository.UpdateCustomerAsync(customer);
                Console.WriteLine("\nCustomer profile updated successfully! Press Enter to return.");
            }
            else
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
            }

            Console.ReadLine();
        }

        // Metod för att visa en kunds orderhistorik där admin kan se alla tidigare beställningar och deras detaljer.
        private async Task ViewCustomerHistoryAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Order History ---");
            Console.WriteLine("(Type '0' to cancel)\n");

            Console.Write("Enter customer ID: ");
            string input = Console.ReadLine();
            if (input == "0") return;

            if (int.TryParse(input, out int customerId))
            {
                var customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);

                if (customer == null)
                {
                    Console.WriteLine("Customer not found! Press Enter to return.");
                }
                else if (customer.Orders == null || !customer.Orders.Any())
                {
                    Console.WriteLine($"\nNo orders found for customer {customer.Email}. Press Enter to return.");
                }
                else
                {
                    Console.WriteLine($"\nOrder history for {customer.Email}:");
                    foreach (var order in customer.Orders)
                    {
                        Console.WriteLine($"- Order #{order.Id} | Date: {order.OrderDate:yyyy-MM-dd} | Total: {order.TotalPrice} kr");

                        // Visa varje rad i ordern med produktnamn och kvantitet.
                        if (order.Rows!= null && order.Rows.Any())
                        {
                            foreach (var row in order.Rows)
                            {
                              
                                string productName = row.Product != null ? row.Product.Name : "Unknown Product";
                                Console.WriteLine($"    * {row.Quantity} x {productName}");
                            }
                        }
                    }
                    Console.WriteLine("\nPress Enter to return.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Press Enter to return.");
            }

            Console.ReadLine();
        }
    }
}