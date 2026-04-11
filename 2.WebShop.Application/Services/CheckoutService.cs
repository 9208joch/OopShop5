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
        private readonly IProductRepository _productRepository;

        public CheckoutService(CartService cartService, IProductRepository productRepository)
        {
            _cartService = cartService;
            _productRepository = productRepository;
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

        public async Task CompleteOrder(IPaymentMethod paymentMethod, decimal totalAmount)
        {
            var cart = _cartService.GetCart();

            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.Product.Id);

                if (product.Inventory < item.Quantity)
                    throw new Exception($"Not enough stock for {product.Name}");

                product.Inventory -= item.Quantity;

                await _productRepository.UpdateProductAsync(product);
            }

            paymentMethod.Pay(totalAmount);

            _cartService.ClearCart();
        }


    }
}
