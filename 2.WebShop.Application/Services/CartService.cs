using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.WebShop.Application.Services
{
    public class CartService
    {

        private ShoppingCart _cart = new();

        public void AddToCart(Product product, int quantity)
        {
            _cart.AddProduct(product, quantity);
        }

        public void RemoveFromCart(Product product)
        {
            _cart.RemoveProduct(product);
        }

        public ShoppingCart GetCart()
        {
            return _cart;
        }

        public void ClearCart()
        {
            _cart = new ShoppingCart();
        }



    }
}
