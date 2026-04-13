using System;
using System.Collections.Generic;
using System.Text;
using _1.WebShop.Core.Entities;

namespace WebShop.ConsoleApp.UI.State
{
public class MenuState
{
    public int SelectedIndex;
    public int SelectedOfferIndex;

    public bool InOffers = true;

    public List<Product> Offers = new();
}
   
}
