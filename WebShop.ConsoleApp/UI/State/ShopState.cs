using System;
using System.Collections.Generic;
using System.Text;
using _1.WebShop.Core.Entities;

namespace WebShop.ConsoleApp.UI.State
{
    public class ShopState
    {
        public List<Product> Offers = new();

        public List<Product> Products = new();

        public List<Product> FilteredProducts = new();

        public DateTime LastTypingTime = DateTime.MinValue;


        public int SelectedOfferIndex;
        public int SelectedProductIndex;
        
        public bool InOffers = true;
        public bool InProducts;
        public bool InSidebar;

        public bool CategoriesOpen;
        public bool CategoryActive;

        public int SelectedSidebarIndex;
        public int SelectedCategoryIndex;

        public string? SelectedCategory;
        public int ProductScrollOffset;

        public bool SearchActive;
        public bool SearchTyping;
        public string SearchQuery = "";

        

        public bool SaleActive;
        public int? SelectedSize;

        public bool InModal;
        public Product? ModalProduct;

        public int SelectedSizeIndex;
        public int SelectedModalButton;

        public int OfferScrollOffset;
    }
}