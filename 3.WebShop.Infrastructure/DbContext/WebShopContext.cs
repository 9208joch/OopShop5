using _1.WebShop.Core.Entities;
using _3.WebShop.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.DbContext
{
    public class WebShopContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public WebShopContext(DbContextOptions<WebShopContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasCheckConstraint("CK_Product_Inventory", "Inventory >= 0");

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Sweater" },
                new Category { Id = 2, Name = "Shorts" },
                new Category { Id = 3, Name = "T-shirt" },
                new Category { Id = 4, Name = "Jeans" },
                new Category { Id = 5, Name = "Jacket" }
            );

            CustomersSeeder.Seed(modelBuilder);
        }
    }
}
