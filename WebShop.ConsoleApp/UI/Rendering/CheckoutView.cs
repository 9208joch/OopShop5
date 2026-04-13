using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _4.WebShop.ConsoleApp.UI.Rendering
{
    public class CheckoutView
    {
        public void ShowSummary(ShoppingCart cart, CheckoutSummary summary)
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

    }
}
