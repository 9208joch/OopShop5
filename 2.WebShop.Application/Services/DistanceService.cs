using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _1.WebShop.Application;
namespace _1.WebShop.Application.Services
{
    
    public class DistanceService : IDistanceService
    {
        // This service calculates the distance from the customer's address to the store using an external API.
      
        private readonly HttpClient _httpClient;
        private const string Apikey = "RPKYd2GNC4FS8QE6fohAf2HMw75orGXtiWaIJTZx";
        private const string StoreAddress = " Kungsgatan 4 451 30 Uddevalla";

        public DistanceService(HttpClient httpClient)
        {
            // In a real application, the API key and store address would likely come from configuration.
          
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("RPKYd2GNC4FS8QE6fohAf2HMw75orGXtiWaIJTZx", Apikey);
        }

        public async Task<double?> GetDistanceToStoreAsync(string customerAddress)
        {
            // This method calls an external API to get the distance from the customer's address to the store.
           
            var url = $"https://api.example.com/distance?from={Uri.EscapeDataString(customerAddress)}&to={Uri.EscapeDataString(StoreAddress)}";

            try
            {
                // Call the external API to get the distance. The actual URL and parameters would depend on the API's requirements.
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<DistanceResponse>();
                    return data?.Distance;
                }
            }
            catch
            {
                // if something goes wrong, we catch the exception and return null 
            }
            return null;
        }

        public class DistanceResponse
        {
            // This class represents the response from the distance API.
            public double Distance { get; set; }
        }
    }
}