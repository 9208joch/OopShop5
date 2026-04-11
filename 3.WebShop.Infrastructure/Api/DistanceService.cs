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

        private const string ApiKey = "DIN_API_KEY_HÄR";/////////////////////////////////////////////////////////////////////////
        private const string StoreAddress = "Kungsgatan 4 451 30 Uddevalla";

        public DistanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double?> GetDistanceToStoreAsync(string _)
        {
            try
            {
                var user = await GetUserLocation();
                var store = await GetCoordinates(StoreAddress);

                if (user == null || store == null)
                    return null;

                return CalculateDistance(user.Value, store.Value);
            }
            catch
            {
                return null;
            }
        }

        //  1. Hämta användarens position via IP
        private async Task<(double lat, double lon)?> GetUserLocation()
        {
            try
            {
                var res = await _httpClient.GetFromJsonAsync<IpResponse>("http://ip-api.com/json/");

                if (res == null || res.Status != "success")
                    return null;

                return (res.Lat, res.Lon);
            }
            catch
            {
                return null;
            }
        }

        //  2. Adress → koordinater
        private async Task<(double lat, double lon)?> GetCoordinates(string address)
        {
            var url = $"https://api.api-ninjas.com/v1/geocoding?address={Uri.EscapeDataString(address)}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Api-Key", ApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<GeoResponse>>(json);

            var first = data?.FirstOrDefault();

            if (first == null)
                return null;

            return (first.Latitude, first.Longitude);
        }

        //  3. Avstånd (Haversine)
        private double CalculateDistance((double lat, double lon) a, (double lat, double lon) b)
        {
            double R = 6371;

            double dLat = ToRad(b.lat - a.lat);
            double dLon = ToRad(b.lon - a.lon);

            double lat1 = ToRad(a.lat);
            double lat2 = ToRad(b.lat);

            double x = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Atan2(Math.Sqrt(x), Math.Sqrt(1 - x));

            return R * c;
        }

        private double ToRad(double deg) => deg * (Math.PI / 180);

        // Models
        private class IpResponse
        {
            public string Status { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        private class GeoResponse
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}