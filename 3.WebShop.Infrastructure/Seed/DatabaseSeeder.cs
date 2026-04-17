using _1.WebShop.Core.Entities;
using _3.WebShop.Infrastructure.Seed;

namespace _3.WebShop.Infrastructure.DbContext
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(WebShopContext context)
        {
      
            // 1. PRODUCTS 
   
            if (context.Products.Any())
            {
                context.Products.RemoveRange(context.Products);
                await context.SaveChangesAsync();
            }

            var products = ProductSeeder.GenerateProducts(200);
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

      
            // 2. CUSTOMERS 
    
            var customers = context.Customers.ToList();

            if (!customers.Any())
            {
                throw new Exception("No customers found in database. Seed customers first.");
            }

 
            // 3. ORDERS 
        
            if (context.Orders.Any())
            {
                context.Orders.RemoveRange(context.Orders);
                await context.SaveChangesAsync();
            }

            var orders = OrderSeeder.GenerateOrders(products, customers, 10);

            Console.WriteLine($"[Seeder] Orders created: {orders.Count}");

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }
}