using _2.WebShop.Application;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _2.WebShop.Application.UseCases;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Api;
using _3.WebShop.Infrastructure.Payments;
using _3.WebShop.Infrastructure.Repositories;
using _3.WebShop.Infrastructure.Shipping;
using _4.WebShop.ConsoleApp.UI.Flows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UI;
using WebShop.ConsoleApp.UI;

var services = new ServiceCollection();

services.AddScoped<ShopMenu>();

services.AddDbContext<WebShopContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));

services.AddScoped<IProductRepository, ProductRepository>();
services.AddSingleton<CartService>();
services.AddScoped<ShoppingCartMenu>();
services.AddScoped<CheckoutService>();

services.AddScoped<CheckoutFlow>();
services.AddScoped<ConsoleCheckoutFlow>();

services.AddScoped<IPaymentMethod, CardPayment>();
services.AddScoped<IPaymentMethod, SwishPayment>();

services.AddScoped<IShippingOption, StandardShipping>();
services.AddScoped<IShippingOption, ExpressShipping>();
services.AddScoped<IShippingOption, NoShippingPickupStore>();
services.AddScoped<AdminMenu>();
services.AddScoped<ICustomerRepository, CustomerRepository>();


services.AddScoped<IOrderRepository, OrderRepository>();        //NK
//var provider = services.BuildServiceProvider();

services.AddScoped<Menu>();
services.AddSingleton<ConsoleNavigationService>();


    services.AddHttpClient<IDistanceService, DistanceService>();

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

    if (repo is ProductRepository concreteRepo)
    {
        await concreteRepo.SeedAsync();
    }

   var menu = scope.ServiceProvider.GetRequiredService<Menu>();
   await menu.Start();

}


namespace WebShop.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<WebShopContext>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<_1.WebShop.Core.Interfaces.ICustomerRepository,
                _3.WebShop.Infrastructure.Repositories.CustomerRepository>();
            
            services.AddScoped<AdminMenu>();
            services.AddSingleton<ConsoleNavigationService>();

            // 1. Bygg ihop alla services
            var serviceProvider = services.BuildServiceProvider();

            // 3. Starta din Admin-meny!
            var adminMenu = serviceProvider.GetRequiredService<AdminMenu>();
            await adminMenu.ShowMenuAsync();
        }
    }
}