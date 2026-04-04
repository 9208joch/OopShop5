using _1.WebShop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2.WebShop.Application.Services.Shipping
{
    public class StandardShipping : IShippingOption
    {
        public string Name
        {
            get { return "Standard"; }
        }

        public decimal Price
        {
            get { return 50m; }
        }
    }
}
