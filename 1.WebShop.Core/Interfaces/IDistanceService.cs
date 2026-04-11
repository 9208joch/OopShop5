using System.Threading.Tasks;
using _1.WebShop.Core.Interfaces;

namespace _1.WebShop.Core.Interfaces
{
    public interface IDistanceService
    {
        Task<double?> GetDistanceToStoreAsync(string customerAddress);
    }
}
    