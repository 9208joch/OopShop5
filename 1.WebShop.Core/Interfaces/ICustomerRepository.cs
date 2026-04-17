using _1.WebShop.Core.Entities;
using System.Threading.Tasks;

namespace _1.WebShop.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<Customer> GetCustomerWithOrdersAsync(int customerId);
        Task UpdateCustomerAsync(Customer customer);




        Task<Customer> GetByEmailAsync(string email);  // NK
    }
}