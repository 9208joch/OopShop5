using _1.WebShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using _1.WebShop.Core.Enums;

namespace _3.WebShop.Infrastructure.Seed
{

    public static class ProductSeeder
    {
        private static Random _rand = new();

        private static string[] categories =
        {
        "Sweater", "Shorts", "T-shirt", "Jeans", "Jacket"
    };

        private static string[] names =
        {
        "Nicklas", "Anita", "Erik", "Sara", "Liam", "Emma",
        "Elin", "Agda", "Sven", "Sote", "Karl", "Bertil", "Astra"
    };

        private static Size[] sizes = Enum.GetValues<Size>();

        private static Dictionary<string, List<string>> descriptions = new()
        {
            ["Sweater"] = new()
        {
            "Grey sweater with floral motif",
            "Soft knitted sweater in wool blend",
            "Oversized sweater with modern fit",
            "Classic sweater with ribbed cuffs",
            "Lightweight sweater perfect for layering"
        },
            ["Shorts"] = new()
        {
            "Comfortable summer shorts",
            "Sporty shorts with breathable fabric",
            "Casual cotton shorts with pockets",
            "Slim fit shorts for warm days",
            "Relaxed fit beach shorts"
        },
            ["T-shirt"] = new()
        {
            "Basic cotton t-shirt",
            "Slim fit t-shirt in soft fabric",
            "Oversized t-shirt with print",
            "Classic crewneck t-shirt",
            "Lightweight everyday t-shirt"
        },
            ["Jeans"] = new()
        {
            "Blue jeans tight model",
            "Regular fit jeans in denim",
            "Slim fit stretch jeans",
            "Relaxed vintage style jeans",
            "High waist jeans with modern cut"
        },
            ["Jacket"] = new()
        {
            "Warm winter jacket",
            "Lightweight spring jacket",
            "Water-resistant outdoor jacket",
            "Classic bomber jacket",
            "Padded jacket for cold weather"
        }
        };

        public static List<Product> GenerateProducts(int count)
        {
            var products = new List<Product>();
            

            while (products.Count < count)
            {
                var category = categories[_rand.Next(categories.Length)];
                var name = names[_rand.Next(names.Length)];

                foreach (var size in sizes)
                {
                    if (products.Count >= count)
                        break;

                    var price = _rand.Next(99, 1500);

                    bool isOnSale = _rand.NextDouble() < 0.1;
                    decimal? salePrice = null;

                    if (isOnSale)
                    {
                        var discount = _rand.Next(10, 51);
                        salePrice = Math.Round(price * (1 - discount / 100m), 2);
                    }

                    int inventory = (_rand.NextDouble() < 0.2)
                        ? 0
                        : _rand.Next(1, 101);

                    products.Add(new Product
                    {
                        
                        Name = $"{category} {name}",
                        Size = size,
                        Price = price,
                        SalePrice = salePrice,
                        IsOnSale = isOnSale,
                        Description = GenerateDescription(category, name),
                        Category = category,
                        Inventory = inventory,
                        Supplier = "local supplier"
                    });
                }
            }

            return products;
        }

        private static string GenerateDescription(string category, string name)
        {
            var list = descriptions[category];
            int index = Math.Abs(name.GetHashCode()) % list.Count;
            return list[index];
        }
    }
}