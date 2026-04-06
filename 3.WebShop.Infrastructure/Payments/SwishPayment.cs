using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.Payments
{
    public class SwishPayment : IPaymentMethod
    {
        public string Name
        {
            get { return "Swish"; }
        }

        public void Pay(decimal amount)
        {
            Console.WriteLine("Enter your 10 digit phone number. (only digits)");
            var phoneNumber = Console.ReadLine();

            while (!IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Not a valid phone number. Enter 10 digits:");
                phoneNumber = Console.ReadLine();
            }


            Console.WriteLine($"Paid {amount} kr with swish on phonenumber: {phoneNumber}");
        }

        private bool IsValidPhoneNumber(string? input)
        {
            return !string.IsNullOrWhiteSpace(input)
               && input.Length == 10 && input.All(char.IsDigit);

        }
    }
}
