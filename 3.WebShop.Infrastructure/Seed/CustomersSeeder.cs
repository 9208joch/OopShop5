using _1.WebShop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3.WebShop.Infrastructure.Seed
{
    public class CustomersSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Anna Andersson",
                    Email = "anna@mail.se",
                    Address = "Storgatan 1",
                    City = "Stockholm",
                    Country = "Sverige",
                    phone = "070-1234567",
                    Age = 28,
                    Gender = "Female",
                    MaskedCreditCard = "1111",
                    OtherContactInfo = "-",
                    PreferredPaymentMethod = "Card"
                },
                new Customer
                {
                    Id = 2,
                    Name = "Erik Karlsson",
                    Email = "erik@mail.se",
                    Address = "Lilla Torget 5",
                    City = "Göteborg",
                    Country = "Sverige",
                    phone = "073-9876543",
                    Age = 45,
                    Gender = "Male",
                    MaskedCreditCard = "2222",
                    OtherContactInfo = "-",
                    PreferredPaymentMethod = "Swish"
                },
                new Customer
                {
                    Id = 3,
                    Name = "Sara Nilsson",
                    Email = "sara@mail.se",
                    Address = "Trädgårdsgatan 12",
                    City = "Malmö",
                    Country = "Sverige",
                    phone = "076-5554433",
                    Age = 32,
                    Gender = "Female",
                    MaskedCreditCard = "3333",
                    OtherContactInfo = "-",
                    PreferredPaymentMethod = "Swish"
                
                }
            );
        }
    }         
    
     
     
}
