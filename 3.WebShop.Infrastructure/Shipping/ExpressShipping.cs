using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.Shipping
{
    public class ExpressShipping : IShippingOption
    {
        public string Name
        {
            get { return "Express"; }
        }

        public decimal Price
        {
            get { return 100m; }
        }

    }
}
