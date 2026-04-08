using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using WebShop.ConsoleApp.UI.State;

namespace WebShop.ConsoleApp.UI.Navigation
{


    public class ShopNavigationHandler
    {
        public async Task HandleSidebarNavigation(
    ShopState state,
    NavigationAction action,
    string[] categories,
    Func<string, Task> loadCategoryProducts,
    Func<Task> loadOffers,
    Func<Task> openCart)
        {
            // DROPDOWN AKTIV
            if (state.CategoriesOpen && state.SelectedSidebarIndex == 1)
            {
                switch (action)
                {
                    case NavigationAction.Up:
                        if (state.SelectedCategoryIndex > 0)
                            state.SelectedCategoryIndex--;
                        break;

                    case NavigationAction.Down:
                        if (state.SelectedCategoryIndex < categories.Length - 1)
                            state.SelectedCategoryIndex++;
                        break;

                    case NavigationAction.Select:
                        state.SelectedCategory = categories[state.SelectedCategoryIndex];
                        state.CategoryActive = true;

                        await loadCategoryProducts(state.SelectedCategory);

                        state.CategoriesOpen = false;
                        state.InSidebar = false;
                        state.InProducts = true;
                        state.InOffers = false;

                        state.ProductScrollOffset = 0;
                        state.SelectedProductIndex = 0;
                        break;

                    case NavigationAction.Back:
                    case NavigationAction.Left:
                        state.CategoriesOpen = false;
                        break;
                }

                return;
            }

            // VANLIG SIDEBAR
            switch (action)
            {
                case NavigationAction.Up:
                    if (state.SelectedSidebarIndex > 0)
                        state.SelectedSidebarIndex--;
                    break;

                case NavigationAction.Down:
                    if (state.SelectedSidebarIndex < 4)
                        state.SelectedSidebarIndex++;
                    break;

                case NavigationAction.Right:
                    state.InSidebar = false;
                    state.InOffers = !state.CategoryActive;
                    state.InProducts = state.CategoryActive;
                    break;

                case NavigationAction.Select:

                    // SALE
                    if (state.SelectedSidebarIndex == 0)
                    {
                        state.SaleActive = true;
                        state.CategoryActive = false;

                        state.OfferScrollOffset = 0;
                        state.SelectedOfferIndex = 0;

                        state.InSidebar = false;
                        state.InOffers = true;
                        state.InProducts = false;

                        await loadOffers();
                        return;
                    }

                    // CATEGORIES
                    if (state.SelectedSidebarIndex == 1)
                    {
                        state.CategoriesOpen = !state.CategoriesOpen;
                        state.SelectedCategoryIndex = 0;
                        return;
                    }

                    // HOME
                    if (state.SelectedSidebarIndex == 2)
                    {
                        state.SaleActive = false;
                        state.CategoryActive = false;
                        state.CategoriesOpen = false;

                        state.InOffers = false;
                        state.InProducts = false;
                        state.InSidebar = false;
                        state.InModal = false;

                        return;
                    }

                    // CART
                    if (state.SelectedSidebarIndex == 3)
                    {
                        await openCart();
                        return;
                    }

                    // SEARCH
                    if (state.SelectedSidebarIndex == 4)
                    {
                        state.SaleActive = false;
                        state.CategoryActive = false;
                        state.CategoriesOpen = false;

                        state.InModal = false;

                        state.SearchActive = true;
                        state.SearchTyping = true;

                        state.SearchQuery = "";

                        state.SelectedProductIndex = 0;
                        state.ProductScrollOffset = 0;

                        state.InSidebar = false;
                        state.InOffers = false;
                        state.InProducts = false;

                        return;
                    }

                    break;
            }
        }
        
        private readonly IProductRepository _repo;
        private readonly CartService _cartService;

        public ShopNavigationHandler(IProductRepository repo, CartService cartService)
        {
            _repo = repo;
            _cartService = cartService;
        }

        public async Task HandleProductsNavigation(
            ShopState state,
            NavigationAction action,
            List<Product> searchResults,
            List<Product> modalVariants)
        {
            var list = state.SearchActive
                ? searchResults
                : (state.CategoryActive ? state.FilteredProducts : state.Products);

            if (list.Count == 0)
                return;

            int cols = 3;

            switch (action)
            {
                case NavigationAction.Left:
                    if (state.SelectedProductIndex % cols > 0)
                    {
                        state.SelectedProductIndex--;
                    }
                    else
                    {
                        state.InSidebar = true;
                        state.InProducts = false;
                    }
                    break;

                case NavigationAction.Right:
                    if (state.SelectedProductIndex % cols < cols - 1 &&
                        state.SelectedProductIndex < list.Count - 1)
                    {
                        state.SelectedProductIndex++;
                    }
                    break;

                case NavigationAction.Up:

                    if (!state.CategoryActive && !state.SaleActive &&
                        state.SelectedProductIndex < cols &&
                        state.ProductScrollOffset == 0)
                    {
                        state.InOffers = true;
                        state.SelectedOfferIndex = 0;
                        return;
                    }

                    if (state.SelectedProductIndex - cols >= 0)
                        state.SelectedProductIndex -= cols;

                    if (state.SelectedProductIndex < state.ProductScrollOffset)
                        state.ProductScrollOffset -= cols;

                    if (state.ProductScrollOffset < 0)
                        state.ProductScrollOffset = 0;

                    break;

                case NavigationAction.Down:

                    if (state.SelectedProductIndex + cols < list.Count)
                        state.SelectedProductIndex += cols;

                    int startY = state.CategoryActive ? 6 : 20;
                    int visibleRows = (Console.WindowHeight - startY) / 4;
                    int maxVisibleItems = visibleRows * cols;

                    if (state.SelectedProductIndex >= state.ProductScrollOffset + maxVisibleItems)
                        state.ProductScrollOffset += cols;

                    break;

                case NavigationAction.Select:

                    if (state.SelectedProductIndex < 0 || state.SelectedProductIndex >= list.Count)
                        return;

                    state.ModalProduct = list[state.SelectedProductIndex];

                    var variants = (await _repo.GetAllAsync())
                        .Where(p => p.Name == state.ModalProduct.Name)
                        .OrderBy(p => p.Size)
                        .ToList();

                    modalVariants.Clear();
                    modalVariants.AddRange(variants);

                    state.SelectedSizeIndex = 0;
                    state.SelectedModalButton = 0;
                    state.SelectedSize = null;

                    state.InModal = true;
                    break;
            }
        }
    }
}