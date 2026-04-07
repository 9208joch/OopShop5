using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class ShoppingCart
    {



        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items;

        public void AddProduct(Product product, int quantity)
        {
            var existingItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
        }

        public void RemoveProduct(Product product)
        {
            _items.RemoveAll(i => i.Product.Id == product.Id);
        }

        public void DecreaseQuantity(Product product, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (item == null) return;

            if (item.Quantity > quantity)
                item.Quantity -= quantity;
            else
                _items.Remove(item);
        }

        public decimal GetTotalPrice()
        {
            return _items.Sum(i => i.Product.Price * i.Quantity);
        }




    }
}
