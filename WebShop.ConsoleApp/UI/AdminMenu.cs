using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _4.WebShop.ConsoleApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.ConsoleApp.UI
{
    public class AdminMenu
    {
        
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ConsoleNavigationService _navigation;

        public AdminMenu(ICustomerRepository customerRepository, IProductRepository productRepository, ConsoleNavigationService navigation)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _navigation = navigation;
        }

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

       
        private async Task ManageProductsAsync()
        {
            var selector = new ListSelector<string>(_navigation);
            var options = new List<string> { "Add New Product", "Edit Product", "Delete Product", "Go Back" };

            var choice = selector.Select(options, item => item, "MANAGE PRODUCTS");

            if (choice == "Add New Product") await AddProductAsync();
            else if (choice == "Edit Product") await EditProductAsync();
            else if (choice == "Delete Product") await DeleteProductAsync();
        }

        private async Task AddProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Product ---");

            Console.Write("Enter product name: ");
            var name = Console.ReadLine();

            Console.Write("Enter product category: ");
            var category = Console.ReadLine();

            Console.Write("Enter product description: ");
            var description = Console.ReadLine();

            Console.Write("Enter product price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price)) price = 0;

            Console.Write("Enter product supplier (Leverantör): ");
            var supplier = Console.ReadLine();

            Console.Write("Enter stock balance (Lagersaldo): ");
            if (!int.TryParse(Console.ReadLine(), out int stockBalance)) stockBalance = 0;

            var newProduct = new Product
            {
                Name = name ?? "Unknown",
                Category = category ?? "Unknown",
                Description = description ?? "Unknown",
                Price = price,
                Supplier = supplier ?? "Unknown",
                StockBalance = stockBalance
            };

            await _productRepository.AddProductAsync(newProduct);
            Console.WriteLine("\nProduct added successfully! Press Enter to return.");
            Console.ReadLine();
        }

        private async Task EditProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Edit Product ---");
            Console.Write("Enter Product ID to edit: ");

            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                var product = await _productRepository.GetProductByIdAsync(productId);

                if (product == null)
                {
                    Console.WriteLine("Product not found! Press Enter.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nEditing: {product.Name} (Leave blank to keep current value)");

                Console.Write($"New Name [{product.Name}]: ");
                var newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;

                Console.Write($"New Price [{product.Price}]: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal newPrice)) product.Price = newPrice;

                await _productRepository.UpdateProductAsync(product);
                Console.WriteLine("\nProduct updated successfully! Press Enter.");
            }
            else Console.WriteLine("Invalid ID. Press Enter.");
            Console.ReadLine();
        }

        private async Task DeleteProductAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Product ---");
            Console.Write("Enter Product ID to delete: ");

            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                await _productRepository.DeleteProductAsync(productId);
                Console.WriteLine("\nProduct deleted! Press Enter.");
            }
            else Console.WriteLine("Invalid ID. Press Enter.");
            Console.ReadLine();
        }

        // customer
        private async Task ManageCustomersAsync()
        {
            var selector = new ListSelector<string>(_navigation);
            var options = new List<string> { "Edit Customer Profile", "View Order History", "Go Back" };

            var choice = selector.Select(options, item => item, "MANAGE CUSTOMERS");

            if (choice == "Edit Customer Profile") await EditCustomerAsync();
            else if (choice == "View Order History") await ViewCustomerHistoryAsync();
        }

        private async Task EditCustomerAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Edit Customer Profile ---");
            Console.Write("Enter customer ID to edit: ");

            if (int.TryParse(Console.ReadLine(), out int customerId))
            {
                var customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);

                if (customer == null)
                {
                    Console.WriteLine("Customer not found! Press Enter.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"\nEditing Customer ID {customer.Id} ({customer.Name})");
                Console.WriteLine("(Leave blank and press Enter to keep current value)");
                Console.WriteLine("---------------------------------------------------");

                Console.Write($"Name [{customer.Name}]: ");
                var newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName)) customer.Name = newName;

                Console.Write($"Email [{customer.Email}]: ");
                var newEmail = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newEmail)) customer.Email = newEmail;

                Console.Write($"Address [{customer.Address}]: ");
                var newAddress = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAddress)) customer.Address = newAddress;

                
                Console.Write($"City [{customer.City}]: ");
                var newCity = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newCity)) customer.City = newCity;

                Console.Write($"Country [{customer.Country}]: ");
                var newCountry = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newCountry)) customer.Country = newCountry;

                Console.Write($"Phone [{customer.phone}]: ");
                var newPhone = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPhone)) customer.phone = newPhone;

                Console.Write($"Age [{customer.Age}]: ");
                var ageInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ageInput) && int.TryParse(ageInput, out int newAge))
                {
                    customer.Age = newAge;
                }

                Console.Write($"Gender [{customer.Gender}]: ");
                var newGender = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newGender)) customer.Gender = newGender;

                Console.Write($"Preferred Payment Method [{customer.PreferredPaymentMethod}]: ");
                var newPayment = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPayment)) customer.PreferredPaymentMethod = newPayment;

                await _customerRepository.UpdateCustomerAsync(customer);
                Console.WriteLine("\nCustomer profile updated successfully! Press Enter.");
            }
            else
            {
                Console.WriteLine("Invalid ID. Press Enter.");
            }

            Console.ReadLine();
        }

        private async Task ViewCustomerHistoryAsync()
        {
            Console.Clear();
            Console.WriteLine("--- Order History ---");
            Console.Write("Enter customer ID: ");

            if (int.TryParse(Console.ReadLine(), out int customerId))
            {
                var customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);
                if (customer == null)
                {
                    Console.WriteLine("Customer not found! Press Enter.");
                }
                else if (customer.Orders == null || !customer.Orders.Any())
                {
                    Console.WriteLine($"\nNo orders found for customer {customer.Email}. Press Enter.");
                }
                else
                {
                    Console.WriteLine($"\nOrder history for {customer.Email}:");
                    foreach (var order in customer.Orders)
                    {
                        Console.WriteLine($"- Order #{order.Id} | Date: {order.OrderDate} | Total: {order.TotalPrice} kr");
                    }
                    Console.WriteLine("\nPress Enter to return.");
                }
            }
            else Console.WriteLine("Invalid ID. Press Enter.");
            Console.ReadLine();
        }
    }
}