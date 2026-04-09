using _1.WebShop.Core.Entities;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasCheckConstraint("CK_Product_Inventory", "Inventory >= 0");
        }
    }
}
