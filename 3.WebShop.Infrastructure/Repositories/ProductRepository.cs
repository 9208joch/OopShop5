using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using _3.WebShop.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace _3.WebShop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebShopContext _context;
        
        public ProductRepository(WebShopContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
            => await _context.Products.ToListAsync();

        public async Task<Product> GetProductByIdAsync(int id)
            => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddProductAsync(Product newProduct)
        {
            await AddAsync(newProduct);
        }

        public async Task SeedAsync()
        {   
            if (_context.Products.Any())

                return;

            var products = ProductSeeder.GenerateProducts(1001);

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
}