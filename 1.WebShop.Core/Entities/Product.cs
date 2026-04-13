
using _1.WebShop.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Enums.Size Size { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; }

        public decimal? SalePrice { get; set; }
        public bool IsOnSale { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }

        [Range(0, int.MaxValue)]
        public int Inventory { get; set; }

        public string Supplier { get; set; }
        public int StockBalance { get; set; }
    }
}
