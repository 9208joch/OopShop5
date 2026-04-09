using _1.WebShop.Core.Interfaces;
using _1.WebShop.Application;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Payments;
using _3.WebShop.Infrastructure.Repositories;
using _3.WebShop.Infrastructure.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UI;
using WebShop.ConsoleApp.UI;
using _2.WebShop.Application.Services;
using _1.WebShop.Application.Services;

var services = new ServiceCollection();

services.AddScoped<ShopMenu>();

services.AddDbContext<WebShopContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));

services.AddScoped<IProductRepository, ProductRepository>();
services.AddSingleton<CartService>();
services.AddScoped<ShoppingCartMenu>();
services.AddScoped<CheckoutService>();

services.AddScoped<IPaymentMethod, CardPayment>();
services.AddScoped<IPaymentMethod, SwishPayment>();

services.AddScoped<IShippingOption, StandardShipping>();
services.AddScoped<IShippingOption, ExpressShipping>();
services.AddScoped<AdminMenu>();
services.AddScoped<ICustomerRepository, CustomerRepository>();

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
            // sätt ihop alla kopplingar i en service collection
            var servicesCollection = new ServiceCollection();

            // koppla database
            servicesCollection.AddDbContext<WebShopContext>();

            // koppla ihop repository-klasserna med service
            servicesCollection.AddScoped<IProductRepository, ProductRepository>();
            servicesCollection.AddScoped<ICustomerRepository, ICustomerRepository>();

            servicesCollection.AddScoped<AdminMenu>();

            // koppla ihop nya menyn med de nya repository-klasserna
            var serviceProvider = servicesCollection.BuildServiceProvider();


            // kör igång adminmenyn
            var adminMenu = serviceProvider.GetRequiredService<AdminMenu>();
            await adminMenu.ShowMenuAsync();

        }
    } 
}

