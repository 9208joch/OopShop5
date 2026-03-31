using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _3.WebShop.Infrastructure.DbContext
{

    public class WebShopContextFactory : IDesignTimeDbContextFactory<WebShopContext>
    {
        public WebShopContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebShopContext>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=WebShopDb;Trusted_Connection=True;");

            return new WebShopContext(optionsBuilder.Options);
        }
    }
}
