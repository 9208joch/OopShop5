using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class CartItem
    {
        public Product Product { get; private set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

    }
}
