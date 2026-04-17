using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebShop.ConsoleApp.UI.State;

public class ShopSidebarNavigationHandler
{
    public async Task Handle(
        ShopState state,
        NavigationAction action,
        string[] categories,
        Func<string, Task> loadCategoryProducts,
        Func<Task> loadOffers,
        Func<Task> openCart)
    {
        // DROPDOWN MODE
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

        // NORMAL SIDEBAR
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

                //  SALE
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

                //  CATEGORIES
                if (state.SelectedSidebarIndex == 1)
                {
                    state.CategoriesOpen = !state.CategoriesOpen;
                    state.SelectedCategoryIndex = 0;
                    return;
                }

                //  HOME
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

                //  CART
                if (state.SelectedSidebarIndex == 3)
                {
                    await openCart();
                    return;
                }

                //  SEARCH
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
}