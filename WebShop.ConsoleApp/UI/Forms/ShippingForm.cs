using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _4.WebShop.ConsoleApp.UI.Forms
{
    public class ShippingForm               // UI kod för namn, adress, postnummer ink valideringar. Retur ShippingInfo objekt.
    {

        public ShippingInfo Collect()
        {
            Console.Clear();
            Console.WriteLine("=== SHIPPING INFO ===\n");

            string name = GetValidatedInput("Name", "Example: Anders Andersson",
                input => !string.IsNullOrWhiteSpace(input),
                "Name cannot be empty.");

            string address = GetValidatedInput("Address", "Example: Exempelvägen 14",
                input => !string.IsNullOrWhiteSpace(input),
                "Address cannot be empty.");

            string zipcode = GetValidatedInput("Zip code", "Example: 45130",
                input => !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit) && input.Length == 5,
                "Zip code must be 5 digits.");

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
    }

}