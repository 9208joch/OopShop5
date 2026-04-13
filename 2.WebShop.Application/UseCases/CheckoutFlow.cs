using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.WebShop.Application.UseCases
{
    public class CheckoutFlow                       // Sammordnar/Orkestrerar CartService och CheckoutService för enklare Checkout process. 
    {

        private readonly CartService _cartService;
        private readonly CheckoutService _checkoutService;

        public CheckoutFlow(
            CartService cartService,
            CheckoutService checkoutService)
        {
            _cartService = cartService;
            _checkoutService = checkoutService;
        }

        public CheckoutSummary CreateSummary(IShippingOption shipping)
        {
            return _checkoutService.CreateSummary(shipping);
        }

        public async Task<Result> CompleteOrder(IPaymentMethod paymentMethod, decimal total)
        {
            return await _checkoutService.CompleteOrder(paymentMethod, total);
        }

        public ShoppingCart GetCart()
        {
            return _cartService.GetCart();
        }



    }
}
