using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class CheckoutSummary
    {
        public decimal SubtotalInclVat { get; set; }
        public decimal SubtotalExVat { get; set; }
        public decimal Vat { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
    }
}
