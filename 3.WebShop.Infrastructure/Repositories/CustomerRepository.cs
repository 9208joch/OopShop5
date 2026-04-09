using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace _3.WebShop.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly WebShopContext _context;

        // 
        public CustomerRepository(WebShopContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerWithOrdersAsync(int customerId)
        {
            return await _context.Customers.
                Include(c => c.Orders).
                FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
           _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
