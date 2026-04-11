using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _4.WebShop.ConsoleApp.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using _3.WebShop.Infrastructure.DbContext;

namespace UI
{
    public class ShoppingCartMenu
    {
        private readonly CartService _cartService;
       

        private readonly IEnumerable<IPaymentMethod> _paymentMethods;
        private readonly IEnumerable<IShippingOption> _shippingOptions;

        private readonly CheckoutService _checkoutService;
        private readonly ConsoleNavigationService _consoleNavigationService;

        private readonly WebShopContext _Context;



        public ShoppingCartMenu(CartService cartService, 
            IEnumerable<IPaymentMethod> paymentMethods,
            IEnumerable<IShippingOption> shippingOptions, CheckoutService checkoutService, ConsoleNavigationService consoleNavigationService, WebShopContext context)
        {
            _cartService = cartService;
            _paymentMethods = paymentMethods;
            _shippingOptions = shippingOptions;
            _checkoutService = checkoutService;
            _consoleNavigationService = consoleNavigationService;
            _Context = context;
        }

        public async Task Start()
        {

            int selectedIndex = 0;
            bool running = true;

            while (running)
            {
                DrawMenu(selectedIndex);

                var action = _consoleNavigationService.GetAction();

                switch (action)
                {
                    case NavigationAction.Up:
                        selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : 4;
                        break;

                    case NavigationAction.Down:
                        selectedIndex = selectedIndex < 4 ? selectedIndex + 1 : 0;
                        break;

                    case NavigationAction.Select:
                        await ExecuteChoice(selectedIndex);
                        if (selectedIndex == 4) 
                            running = false;

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task Checkout()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            // Fråga Efter Kunden 
            Console.WriteLine("\nKund-Id : ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("");
                Console.ReadKey();
                return;
            }

            var customer = await _Context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                Console.WriteLine("customer ");
                Console.ReadKey();
                return;
            }

            var shippingInfo = GetShippingInfo();
            var shipping = SelectShipping();

            if (shipping == null)
            {
                Console.WriteLine("Shipping selection cancelled.");
                Console.ReadKey();
                return;
            }

            var summary = _checkoutService.CreateSummary(shipping);

            ShowSummary(cart, summary);

            Console.WriteLine("\nPress ENTER to continue to payment or BACKSPACE to cancel...");

            var action = _consoleNavigationService.GetAction();

            if (action == NavigationAction.Back)
            {
                Console.WriteLine("Checkout cancelled.");
                Console.ReadKey();
                return;
            }

            var paymentMethod = SelectPaymentMethod();

            if (paymentMethod == null)
            {
                Console.WriteLine("Payment selection cancelled.");
                Console.ReadKey();
                return;
            }


            await _checkoutService.CompleteOrder(paymentMethod, summary.Total);


            //  
            var newOrder = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                TotalPrice = summary.Total,
                Rows = new List<OrderRow>()
            };
            foreach (var item in cart.Items)
            {
                newOrder.Rows.Add(new OrderRow { ProductId = item.Product.Id, Quantity = item.Quantity });
            }

            _Context.Orders.Add(newOrder);
            await _Context.SaveChangesAsync();

            foreach (var item in cart.Items.ToList())
            {
                // töma varuKorgen för kunden
                _cartService.RemoveFromCart(item.Product);
            }


            Console.WriteLine("Order completed");

        }

        private void ShowSummary(ShoppingCart cart, CheckoutSummary summary)
        {
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
        }


        private void DrawMenu(int selectedIndex)
        {
            string[] options =
            {
                
                "View cart",
                "Update quantity",
                "Remove product",
                "Checkout",
                "Exit"
            };

            Console.Clear();
            Console.WriteLine("=== WEBSHOP SHOPPING CART ===\n");

            for (int i = 0; i < options.Length; i++)
            {
                bool selected = i == selectedIndex;

                if (selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);

                Console.ResetColor(); 
            }
        }

        private async Task ExecuteChoice(int index)
        {

            switch (index)
            {
                case 0:
                    ShowCart();
                    break;

                case 1:
                    UpdateQuantity();
                    break;

                case 2:
                    RemoveFromCart();
                    break;

                case 3:
                    await Checkout();
                    break;

                case 4:
                    break;

            }
        }

        private int EditQuantity(int startValue)
        {
            int quantity = startValue;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== EDIT QUANTITY ===\n");

                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine($"Quantity: {quantity}".PadRight(30));
                Console.ResetColor();

                Console.WriteLine("\nUP = +1 | DOWN = -1");
                Console.WriteLine("ENTER = confirm | BACKSPACE = cancel");

                var action = _consoleNavigationService.GetAction();

                switch (action)
                {
                    case NavigationAction.Up:
                        quantity++;
                        break;

                    case NavigationAction.Down:
                        if (quantity > 1)
                            quantity--;
                        break;

                    case NavigationAction.Select:
                        return quantity;

                    case NavigationAction.Back:
                        return startValue;
                }
            }
        }

        private IShippingOption SelectShipping()
        {
            var options = _shippingOptions.ToList();

            var selector = new ListSelector<IShippingOption>(_consoleNavigationService);

            var selected = selector.Select(
                options,
                s => $"{s.Name} ({s.Price} kr)",
                "Shipping options"
            );

            return selected;

        }

        private ShippingInfo GetShippingInfo()
        {
            Console.Clear();
            Console.WriteLine("=== SHIPPING INFO ===\n");

            string name = GetValidatedInput(
                "Name",
                "Example: Anders Andersson",
                input => !string.IsNullOrWhiteSpace(input),
                "Name cannot be empty."
            );

            string address = GetValidatedInput(
                "Address",
                "Example: Exempelvägen 14",
                input => !string.IsNullOrWhiteSpace(input),
                "Address cannot be empty."
            );

            string zipcode = GetValidatedInput(
                "Zip code",
                "Example: 45130",
                input => !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit) && input.Length == 5,
                "Zip code must be 5 digits."
            );

            return new ShippingInfo
            {
                Name = name,
                Address = address,
                Zipcode = zipcode
            };
        }

        private string GetValidatedInput(string label, string example, Func<string, bool> validator, string errorMessage)
        {
            while (true)
            {
                Console.WriteLine($"\n{label}");
                Console.WriteLine(example);
                Console.Write(": ");

                var input = Console.ReadLine();

                if (validator(input))
                    return input;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMessage);
                Console.ResetColor();
            }
        }



        private IPaymentMethod SelectPaymentMethod()
        {

            var methods = _paymentMethods.ToList();

            var selector = new ListSelector<IPaymentMethod>(_consoleNavigationService);

            var selected = selector.Select(
                methods,
                m => m.Name,
                "Payment methods"
            );

            return selected;


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
                Console.ReadKey();
                return;
            }

            var selector = new ListSelector<CartItem>(_consoleNavigationService);

            var selectedItem = selector.Select(
                cart.Items.ToList(),
                item => $"{item.Product.Name} x {item.Quantity}",
                "Update quantity"
            );

            
            if (selectedItem == null)
                return;

            int newQuantity = EditQuantity(selectedItem.Quantity);

            
            _cartService.RemoveFromCart(selectedItem.Product);
            _cartService.AddToCart(selectedItem.Product, newQuantity);

            Console.WriteLine("\nQuantity updated!");
            Console.ReadKey();



        }

        private void RemoveFromCart()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                Console.ReadKey();
                return;
            }

            var selector = new ListSelector<CartItem>(_consoleNavigationService);

            var selectedItem = selector.Select(
                cart.Items.ToList(),
                item => $"{item.Product.Name} x {item.Quantity}",
                "Remove product"
            );

            
            if (selectedItem == null)
                return;

            _cartService.RemoveFromCart(selectedItem.Product);

            Console.WriteLine("\nProduct removed!");
            Console.ReadKey();
        }



    }
}