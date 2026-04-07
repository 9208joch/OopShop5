using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Interfaces
{
    public interface IShippingOption
    {
        string Name { get; }

        decimal Price { get; }

    }
}
