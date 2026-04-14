using _1.WebShop.Core.Entities;
using _3.WebShop.Infrastructure.Seed;

namespace _3.WebShop.Infrastructure.DbContext
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(WebShopContext context)
        {
            //  1. PRODUCTS
            if (!context.Products.Any())
            {
                var Products = ProductSeeder.GenerateProducts(200);
                context.Products.AddRange(Products);
                await context.SaveChangesAsync();
            }

            //  2. CUSTOMERS (måste finnas)
            var customers = context.Customers.ToList();

            if (!customers.Any())
            {
                throw new Exception("No customers found in database. Seed customers first.");
            }

            //  3. HÄMTA PRODUCTS FRÅN DB (med riktiga Id:n)
            var products = context.Products.ToList();

            //  4. ORDERS
            if (!context.Orders.Any())
            {
                var orders = OrderSeeder.GenerateOrders(products, customers, 10);

                Console.WriteLine($"[Seeder] Orders created: {orders.Count}");

                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();
            }
        }
    }
}