using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
 

    public class OrderRow
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
