using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }


        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal GetTotalPrice()
        {
            return Product.Price * Quantity;
        }



    }
}
