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

        public ConsoleCheckoutFlow(
            CheckoutFlow checkoutFlow,
            IEnumerable<IPaymentMethod> paymentMethods,
            IEnumerable<IShippingOption> shippingOptions,
            ConsoleNavigationService nav)
        {
            _checkoutFlow = checkoutFlow;
            _paymentMethods = paymentMethods;
            _shippingOptions = shippingOptions;
            _nav = nav;

            _shippingForm = new ShippingForm();
            _view = new CheckoutView();
        }

        public async Task Run()
        {
            var cart = _checkoutFlow.GetCart();

            if (!cart.Items.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            var shippingInfo = _shippingForm.Collect();

            var shipping = SelectShipping();
            if (shipping == null) return;

            var summary = _checkoutFlow.CreateSummary(shipping);

            _view.ShowSummary(cart, summary);

            Console.WriteLine("\nPress ENTER to continue or BACKSPACE to cancel...");

            var action = _nav.GetAction();
            if (action == NavigationAction.Back) return;

            var payment = SelectPaymentMethod();
            if (payment == null) return;

           Result result = await _checkoutFlow.CompleteOrder(payment, summary.Total);

            if (!result.Success)
            {
                Console.WriteLine(result.Message);
            }
            else
            {
                Console.WriteLine(result.Message);
            }
                    

            //Console.WriteLine("Order completed!");
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
