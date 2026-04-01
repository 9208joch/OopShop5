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
