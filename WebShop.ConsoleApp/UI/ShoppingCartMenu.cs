using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class ShoppingCartMenu
    {
        private readonly CartService _cartService;
        private readonly IProductRepository _productRepository;

        private readonly IEnumerable<IPaymentMethod> _paymentMethods;
        private readonly IEnumerable<IShippingOption> _shippingOptions;

        private readonly CheckoutService _checkoutService;

        public ShoppingCartMenu(CartService cartService, IProductRepository productRepository,
            IEnumerable<IPaymentMethod> paymentMethods,
            IEnumerable<IShippingOption> shippingOptions, CheckoutService checkoutService)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _paymentMethods = paymentMethods;
            _shippingOptions = shippingOptions;
            _checkoutService = checkoutService;
        }

        public async Task Start()
        {
            var products = await _productRepository.GetAllAsync();

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== WEBSHOP SHOPPING CART ===");
                Console.WriteLine("1. View products");
                Console.WriteLine("2. Add to cart");
                Console.WriteLine("3. View cart");
                Console.WriteLine("4. Update quantity");
                Console.WriteLine("5. Remove product");
                Console.WriteLine("6. Checkout");
                Console.WriteLine("9. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowProducts(products);
                        break;

                    case "2":
                        AddToCart(products);
                        break;

                    case "3":
                        ShowCart();
                        break;

                    case "4":
                        UpdateQuantity();
                        break;

                    case "5":
                        RemoveFromCart();
                        break;

                    case "6":
                        Checkout();
                        break;

                    case "9":
                        running = false;
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void Checkout()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            var shippingInfo = GetShippingInfo();
            var shipping = SelectShipping();

            var summary = _checkoutService.CreateSummary(shipping);

            Console.WriteLine("\n=== ORDER SUMMARY ===");

            foreach (var item in cart.Items)
            {
                Console.WriteLine($"{item.Product.Name} x {item.Quantity}");
            }

            Console.WriteLine("----------------------");
            Console.WriteLine($"Subtotal (ex VAT): {summary.SubtotalExVat:F2} kr");
            Console.WriteLine($"VAT (25%): {summary.Vat:F2} kr");
            Console.WriteLine($"Shipping: {summary.ShippingPrice:F2} kr");
            Console.WriteLine($"TOTAL: {summary.Total:F2} kr");

            var paymentMethod = SelectPaymentMethod();

            _checkoutService.CompleteOrder(paymentMethod, summary.Total);

            Console.WriteLine("Order completed");

        }

        private IShippingOption SelectShipping()
        {
            Console.WriteLine("\n=== SHIPPING OPTIONS ===");

            var options = _shippingOptions.ToList();

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i].Name} ({options[i].Price} kr)");
            }

            int choice = int.Parse(Console.ReadLine()) - 1;

            return options[choice];
        }

        private ShippingInfo GetShippingInfo()
        {
            Console.WriteLine("\n=== SHIPPING INFO ===");

            Console.Write("Name: ");
            var name = Console.ReadLine();

            Console.Write("Address: ");
            var address = Console.ReadLine();

            return new ShippingInfo
            {
                Name = name,
                Address = address
            };
        }


        private IPaymentMethod SelectPaymentMethod()
        {
            Console.WriteLine("\n=== PAYMENT METHODS ===");

            var methods = _paymentMethods.ToList();

            for (int i = 0; i < methods.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {methods[i].Name}");
            }

            int choice = int.Parse(Console.ReadLine()) - 1;

            return methods[choice];
        }

        private void ShowProducts(List<Product> products)
        {
            Console.WriteLine("\n=== PRODUCTS ===");

            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                Console.WriteLine($"{i + 1}. {p.Name} - {p.Price} kr");
            }
        }

        private void AddToCart(List<Product> products)
        {
            ShowProducts(products);

            Console.Write("Select product: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            Console.Write("Quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            var product = products[index];

            _cartService.AddToCart(product, quantity);

            Console.WriteLine("Product added to cart!");
        }

        private void ShowCart()
        {
            var cart = _cartService.GetCart();

            Console.WriteLine("\n=== SHOPPING CART ===");

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            int i = 1;

            foreach (var item in cart.Items)
            {
                var total = item.Product.Price * item.Quantity;

                Console.WriteLine($"{i}. {item.Product.Name} x {item.Quantity} = {total} kr");
                i++;
            }

            Console.WriteLine("----------------------");
            Console.WriteLine($"TOTAL: {cart.GetTotalPrice()} kr");
        }

        private void UpdateQuantity()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            ShowCart();

            Console.Write("Select product: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            var item = cart.Items[index];

            Console.Write("New quantity: ");
            int newQuantity = int.Parse(Console.ReadLine());

            _cartService.RemoveFromCart(item.Product);
            _cartService.AddToCart(item.Product, newQuantity);

            Console.WriteLine("Quantity updated!");
        }

        private void RemoveFromCart()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            ShowCart();

            Console.Write("Select product to remove: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            var item = cart.Items[index];

            _cartService.RemoveFromCart(item.Product);

            Console.WriteLine("Product removed!");
        }
    }
}