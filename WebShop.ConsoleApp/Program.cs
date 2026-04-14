using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _2.WebShop.Application.UseCases;
using _3.WebShop.Infrastructure.Api;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Payments;
using _3.WebShop.Infrastructure.Repositories;
using _3.WebShop.Infrastructure.Shipping;
using _4.WebShop.ConsoleApp.UI.Flows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UI;
using WebShop.ConsoleApp.UI;

namespace WebShop.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            //  DbContext
            services.AddDbContext<WebShopContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));

            //  Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            //  Services
            services.AddSingleton<CartService>();
            services.AddScoped<CheckoutService>();

            //  Flows
            services.AddScoped<CheckoutFlow>();
            services.AddScoped<ConsoleCheckoutFlow>();

            //  Payment
            services.AddScoped<IPaymentMethod, CardPayment>();
            services.AddScoped<IPaymentMethod, SwishPayment>();

            //  Shipping
            services.AddScoped<IShippingOption, StandardShipping>();
            services.AddScoped<IShippingOption, ExpressShipping>();
            services.AddScoped<IShippingOption, NoShippingPickupStore>();

            //  UI
            services.AddScoped<Menu>();
            services.AddScoped<ShopMenu>();
            services.AddScoped<ShoppingCartMenu>();
            services.AddScoped<AdminMenu>();
            services.AddSingleton<ConsoleNavigationService>();

            //  API
            services.AddHttpClient<IDistanceService, DistanceService>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WebShopContext>();


                //  Seed database
                await DatabaseSeeder.SeedAsync(context);

                //  Starta appen
                var menu = scope.ServiceProvider.GetRequiredService<Menu>();
                await menu.Start();
            }
        }
    }
}