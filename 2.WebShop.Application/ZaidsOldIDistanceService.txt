using System.Threading.Tasks;


namespace _1.WebShop.Application
{
    public interface IDistanceService
    {
        // This method calculates the distance from the customer's address to the store.
        Task<double?> GetDistanceToStoreAsync(string customerAddress);
    }
}
    