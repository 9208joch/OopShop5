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

        private readonly IOrderRepository _orderRepository;    //NK

        public CheckoutService(CartService cartService, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
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

        public async Task<Result> CompleteOrder(IPaymentMethod paymentMethod, decimal totalAmount, Customer customer)
        {
            var cart = _cartService.GetCart();

            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.Product.Id);

                if (product.Inventory < item.Quantity)
                {
                    return Result.Fail($"Not enough stock for {product.Name}");
                }


                product.Inventory -= item.Quantity;

                await _productRepository.UpdateProductAsync(product);
            }

            paymentMethod.Pay(totalAmount);

            
            var order = new Order
            {
                CustomerId = customer?.Id, 
                OrderDate = DateTime.Now,
                TotalPrice = totalAmount,

                Rows = cart.Items.Select(i => new OrderRow
                {
                    ProductId = i.Product.Id,
                    Quantity = i.Quantity,
                                               
                }).ToList()
            };

            
            await _orderRepository.AddOrderAsync(order);


            _cartService.ClearCart();

            return Result.Ok("Order completed successfully");

        }


    }
}
