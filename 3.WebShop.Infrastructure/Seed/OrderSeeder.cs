using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _3.WebShop.Infrastructure.Seed
{
    public static class OrderSeeder
    {
        private static Random _rand = new();

        public static List<Order> GenerateOrders(
            List<Product> products,
            List<Customer> customers,
            int orderCount = 10)
        {
            var orders = new List<Order>();

            for (int i = 0; i < orderCount; i++)
            {
                var customer = customers[_rand.Next(customers.Count)];

                int numberOfProducts = _rand.Next(1, 11);

                var selectedProducts = products
                    .OrderBy(x => _rand.Next())
                    .Take(numberOfProducts)
                    .ToList();

                var rows = new List<OrderRow>();

                foreach (var product in selectedProducts)
                {
                    int quantity = _rand.Next(1, 5);

                    rows.Add(new OrderRow
                    {
                        Product = product,   //  viktigt!
                        Quantity = quantity
                    });
                }

                var totalPrice = rows.Sum(r =>
                {
                    var price = r.Product.IsOnSale && r.Product.SalePrice.HasValue
                        ? r.Product.SalePrice.Value
                        : r.Product.Price;

                    return price * r.Quantity;
                });

                orders.Add(new Order
                {
                    CustomerId = customer.Id,
                    CreatedAt = DateTime.Now,
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    Rows = rows
                });
            }

            return orders;
        }
    }
}