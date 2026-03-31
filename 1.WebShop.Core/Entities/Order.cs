using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public List<OrderRow> Rows { get; set; } = new();

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
