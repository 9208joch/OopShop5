using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Product> Products { get; set; } = new();
    }
}
