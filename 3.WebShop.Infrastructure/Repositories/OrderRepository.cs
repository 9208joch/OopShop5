using System;
using System.Collections.Generic;
using System.Text;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _3.WebShop.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace _3.WebShop.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WebShopContext _context;

        public OrderRepository(WebShopContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        //  NY METOD
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Rows)
                .ThenInclude(r => r.Product)
                .ToListAsync();
        }
    }
}