using System;
using System.Collections.Generic;
using System.Text;

namespace _1.WebShop.Core.Entities
{
    public class Customer
    {
       public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        
        public string phone { get; set; }
        public string OtherContactInfo { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        // masked credit card number, for example: **** **** **** 1234, and payment method: MasterCard, swich.
        public string MaskedCreditCard { get; set; }
         
        public string PreferredPaymentMethod { get; set; }

        public ICollection<Order> Orders { get; set; }=new List<Order>();
    }
}
