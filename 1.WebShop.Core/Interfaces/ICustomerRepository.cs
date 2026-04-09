using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Interfaces
{
    public interface ICustomerRepository
    {
        // order repository, customer with orders.
        Task<Customer?> GetCustomerWithOrdersAsync (int customerId);

        // change the information of the customer.
        Task UpdateCustomerAsync(Customer customer);
    }
}
