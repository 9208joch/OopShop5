using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.WebShop.Application.Services
{
    public class CheckoutService
    {

        private readonly CartService _cartService;

        public CheckoutService(CartService cartService)
        {
            _cartService = cartService;
        }

        public CheckoutSummary CreateSummary(IShippingOption shipping)
        {
            var cart = _cartService.GetCart();

            decimal subtotalInclVat = cart.GetTotalPrice();

            decimal priceExVat = subtotalInclVat / 1.25m;
            decimal vat = subtotalInclVat - priceExVat;

            decimal total = subtotalInclVat + shipping.Price;

            return new CheckoutSummary
            {
                SubtotalInclVat = subtotalInclVat,
                SubtotalExVat = priceExVat,
                Vat = vat,
                ShippingPrice = shipping.Price,
                Total = total
            };
        }

        public void CompleteOrder(IPaymentMethod paymentMethod, decimal totalAmount)
        {
            paymentMethod.Pay(totalAmount);
            _cartService.ClearCart();
        }


    }
}
