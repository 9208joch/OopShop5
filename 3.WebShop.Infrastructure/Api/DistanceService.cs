using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _2.WebShop.Application;
using System.Text.Json;
using _1.WebShop.Core.Interfaces;

namespace _3.WebShop.Infrastructure.Api
{
    public class DistanceService : IDistanceService
    {
        private readonly HttpClient _httpClient;

        // Butikens koordinater (Uddevalla)
        private readonly (double lat, double lon) _storeLocation = (58.3473, 11.9424);

        public DistanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double?> GetDistanceToStoreAsync(string _)
        {
            try
            {
                var userLocation = await GetUserLocation();

                // enligt uppgift pdf så skall det finnas "Http-klient till valfri API" vi använder därför IP-geolocation API för att hämta användarens position och beräknar sedan avstånd till vår butik med Haversine-formel.
                // bristen att göra på detta vis är att den räknar från närmaste nod i mitt fall är detta en telefonstolpe så för mig så visar det ca 200m kortare en det faktist är
                if (userLocation == null)
                    return null;

                return CalculateDistance(userLocation.Value, _storeLocation);
            }
            catch
            {
                return null;
            }
        }

        //  Hämta användarens position via IP
        private async Task<(double lat, double lon)?> GetUserLocation()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IpResponse>("http://ip-api.com/json/");

                if (response == null || response.Status != "success")
                    return null;

                return (response.Lat, response.Lon);
            }
            catch
            {
                return null;
            }
        }

        //  Beräkna avstånd (Haversine-formel)
        private double CalculateDistance((double lat, double lon) a, (double lat, double lon) b)
        {
            const double earthRadiusKm = 6371;

            double dLat = ToRadians(b.lat - a.lat);
            double dLon = ToRadians(b.lon - a.lon);

            double lat1 = ToRadians(a.lat);
            double lat2 = ToRadians(b.lat);

            double x = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));

            return earthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        //  API-svar
        private class IpResponse
        {
            public string Status { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}