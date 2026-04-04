using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Interfaces
{
    public interface IPaymentMethod
    {
        string Name {  get; }

        void Pay(decimal amount);

    }
}
