using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using System.Threading.Tasks;

namespace _2.WebShop.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepo;

        public CustomerService(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
        {
            return await _customerRepo.GetCustomerWithOrdersAsync(customerId);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepo.UpdateCustomerAsync(customer);
        }
    }
}