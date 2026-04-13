using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.Shipping
{
    public class NoShippingPickupStore : IShippingOption
    {
        public string Name
        {
            get { return "Pick up in store"; }
        }

        public decimal Price
        {
            get { return 0m; }
        }
    }
}
