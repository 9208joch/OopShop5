using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Payments;
using _3.WebShop.Infrastructure.Repositories;
using _3.WebShop.Infrastructure.Shipping;
using _4.WebShop.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContext<WebShopContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));

services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<CartService>();
services.AddScoped<MenuService>();
services.AddScoped<CheckoutService>();


services.AddScoped<IPaymentMethod, CardPayment>();
services.AddScoped<IPaymentMethod, SwishPayment>();

services.AddScoped<IShippingOption, StandardShipping>();
services.AddScoped<IShippingOption, ExpressShipping>();


var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

    if (repo is ProductRepository concreteRepo)
    {
        await concreteRepo.SeedAsync();
    }

    var menu = scope.ServiceProvider.GetRequiredService<MenuService>();

    await menu.RunAsync();
}