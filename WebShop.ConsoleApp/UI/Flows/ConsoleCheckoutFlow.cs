using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application;
using _2.WebShop.Application.UseCases;
using _4.WebShop.ConsoleApp.UI.Forms;
using _4.WebShop.ConsoleApp.UI.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace _4.WebShop.ConsoleApp.UI.Flows
{
    public class ConsoleCheckoutFlow                // UI controller för checkout i konsolen. Hur användaren går igenom flera UI steg flödet
    {
        private readonly CheckoutFlow _checkoutFlow;
        private readonly IEnumerable<IPaymentMethod> _paymentMethods;
        private readonly IEnumerable<IShippingOption> _shippingOptions;
        private readonly ConsoleNavigationService _nav;

        private readonly ShippingForm _shippingForm;
        private readonly CheckoutView _view;



        private readonly ICustomerRepository _customerRepository;  // NK

        public ConsoleCheckoutFlow(
            CheckoutFlow checkoutFlow,
            IEnumerable<IPaymentMethod> paymentMethods,
            IEnumerable<IShippingOption> shippingOptions,
            ConsoleNavigationService nav, ICustomerRepository customerRepository)
        {
            _checkoutFlow = checkoutFlow;
            _paymentMethods = paymentMethods;
            _shippingOptions = shippingOptions;
            _nav = nav;

            _shippingForm = new ShippingForm();
            _view = new CheckoutView();
            _customerRepository = customerRepository;
        }

        public async Task Run()
        {
            var cart = _checkoutFlow.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            // välj checkout typ med ListSelector
            Customer customer = null;
            ShippingInfo shippingInfo = null;

            var selector = new ListSelector<string>(_nav);

            var options = new List<string>
            {
                "Guest checkout",
                "Existing customer"
            };

            var selected = selector.Select(options, x => x, "Checkout type");

            if (selected == null)
                return;

            if (selected == "Guest checkout")
            {
                shippingInfo = _shippingForm.Collect();
            }
            else if (selected == "Existing customer")
            {
                Console.Clear();
                Console.Write("Enter email: ");
                var email = Console.ReadLine();

                customer = await _customerRepository.GetByEmailAsync(email);

                if (customer == null)
                {
                    Console.WriteLine("Customer not found.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nCustomer: {customer.Name}");
                Console.WriteLine($"Address: {customer.Address}, {customer.City}");
                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();

                shippingInfo = new ShippingInfo
                {
                    Name = customer.Name,
                    Address = customer.Address,
                    Zipcode = ""
                };
            }

            
            var shipping = SelectShipping();
            if (shipping == null) return;

            var summary = _checkoutFlow.CreateSummary(shipping);

            _view.ShowSummary(cart, summary);

            Console.WriteLine("\nPress ENTER to continue or BACKSPACE to cancel...");

            var action = _nav.GetAction();
            if (action == NavigationAction.Back) return;

            var payment = SelectPaymentMethod();
            if (payment == null) return;

            Result result = await _checkoutFlow.CompleteOrder(payment, summary.Total, customer);

            Console.WriteLine(result.Message);
        }

        private IShippingOption SelectShipping()
        {
            var selector = new ListSelector<IShippingOption>(_nav);

            return selector.Select(
                _shippingOptions.ToList(),
                s => $"{s.Name} ({s.Price} kr)",
                "Shipping options");
        }

        private IPaymentMethod SelectPaymentMethod()
        {
            var selector = new ListSelector<IPaymentMethod>(_nav);

            return selector.Select(
                _paymentMethods.ToList(),
                m => m.Name,
                "Payment methods");
        }


    }
}
