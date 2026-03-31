using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var services = new ServiceCollection();


services.AddDbContext<WebShopContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;"));


services.AddScoped<IProductRepository, ProductRepository>();

var provider = services.BuildServiceProvider();
// testkod
using (var scope = provider.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

    if (repo is ProductRepository concreteRepo)
    {
       
        await concreteRepo.SeedAsync();


        var products = await concreteRepo.GetAllAsync();

        Console.WriteLine("=== PRODUCTS ===\n");

        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id} | {p.Name} ({p.Size})");
            Console.WriteLine($"Price: {p.Price} kr");
            Console.WriteLine($"Stock: {p.Inventory}");
            Console.WriteLine("----------------------");
        }
    }
}

Console.WriteLine("\nTryck valfri knapp...");
Console.ReadKey();