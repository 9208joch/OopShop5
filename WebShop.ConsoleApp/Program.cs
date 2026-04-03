using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebShop.ConsoleApp.UI;


var services = new ServiceCollection();
services.AddScoped<ShopMenu>();


services.AddDbContext<WebShopContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));


services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<Menu>();
services.AddSingleton<ConsoleNavigationService>();

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
