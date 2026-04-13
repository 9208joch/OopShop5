using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _4.WebShop.ConsoleApp.UI;
using _4.WebShop.ConsoleApp.UI.Flows;
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
        private readonly ConsoleNavigationService _consoleNavigationService;
        private readonly ConsoleCheckoutFlow _checkoutFlow;

        private readonly WebShopContext _Context;



        public ShoppingCartMenu(CartService cartService, 
            ConsoleNavigationService consoleNavigationService, ConsoleCheckoutFlow checkoutFlow)
        {
            _cartService = cartService;
            _consoleNavigationService = consoleNavigationService;
            _checkoutFlow = checkoutFlow;
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
                    await _checkoutFlow.Run();
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