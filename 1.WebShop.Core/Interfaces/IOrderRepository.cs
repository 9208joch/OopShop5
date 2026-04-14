using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Interfaces
{
    public interface IOrderRepository           //NK
    {
        Task AddOrderAsync(Order order);        

    }
}
