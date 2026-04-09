using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task AddProductAsync(Product newProduct);
    }
}
