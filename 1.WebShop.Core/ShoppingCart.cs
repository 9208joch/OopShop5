using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core
{
    public class ShoppingCart
    {

        private List<CartItem> items = new();

        public void AddProduct(Product product, int quantity) 
        {
            var existingItem = items
                .FirstOrDefault(i => i.Product == product);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem(product, quantity));
            }

        }

        public void RemoveProduct(Product product) 
        {
            items.RemoveAll(i => i.Product == product);
        }

        public void DecreaseProductQuantity(Product product, int quantity)
        {

            var item = items.FirstOrDefault(i => i.Product == product);

            if (item != null)
            {
                if (item.Quantity > quantity)
                {
                    item.Quantity -= quantity;  
                }
                else
                {
                    items.Remove(item);
                }
            }

        }

        public decimal GetTotalPrice() 
        {
            return items.Sum(i => i.GetTotalPrice());

        }

        public void ShowCart()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Console.WriteLine(i + 1 + ". " + item.Product.Name + " Antal: " + item.Quantity);
            }

            Console.WriteLine("Total: " + GetTotalPrice());

        }

        public List<CartItem> GetItems()
        {
            return items;
        }

    }
}
