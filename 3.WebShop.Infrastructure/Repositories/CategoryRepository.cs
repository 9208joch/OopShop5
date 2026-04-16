using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace _3.WebShop.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WebShopContext _context;

        public CategoryRepository(WebShopContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
    }
}