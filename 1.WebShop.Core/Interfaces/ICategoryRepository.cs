using System;
using System.Collections.Generic;
using System.Text;
using _1.WebShop.Core.Entities;

namespace _1.WebShop.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task AddAsync(Category category);
    }
}
