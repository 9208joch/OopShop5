using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.Payments
{
    public class CardPayment : IPaymentMethod
    {
        public string Name
        {
            get { return "Card"; }
        }

        public void Pay(decimal amount)
        {
            Console.WriteLine("Enter your card number (16 digits) :");
            var cardNumber = Console.ReadLine();

            while (!IsValidCardNumber(cardNumber))
            {
                Console.WriteLine("Wrong card number. Enter 16 digits: ");
                cardNumber = Console.ReadLine();
            }

            Console.WriteLine($"Paid {amount} kr on card {cardNumber}");
        }

        public bool IsValidCardNumber(string? input)
        {
            return !string.IsNullOrWhiteSpace(input)
                   && input.Length == 16 && input.All(char.IsDigit);

        }


    }
}
