using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _1.WebShop.Core.Entities;
using _2.WebShop.Application.Services;
using WebShop.ConsoleApp.UI.State;

public class ShopModalNavigationHandler
{
    private readonly CartService _cartService;
    private readonly ConsoleNavigationService _nav;

    public ShopModalNavigationHandler(
        CartService cartService,
        ConsoleNavigationService nav)
    {
        _cartService = cartService;
        _nav = nav;
    }

    public async Task Handle(
        ShopState state,
        NavigationAction action,
        List<Product> modalVariants,
        Action resetModal,
        Action triggerSizeError)
    {
        switch (action)
        {
            case NavigationAction.Select:

                // välj storlek
                if (state.SelectedModalButton == 0)
                {
                    state.SelectedSize = state.SelectedSizeIndex;
                    return;
                }

                // Add to cart
                if (state.SelectedModalButton == 1)
                {
                    if (state.SelectedSize == null)
                    {
                        triggerSizeError();
                        return;
                    }

                    var selected = modalVariants[state.SelectedSize.Value];

                    if (selected.Inventory == 0)
                    {
                        triggerSizeError();
                        return;
                    }

                    _cartService.AddToCart(selected, 1);

                    Console.Clear();
                    Console.WriteLine($"Added {selected.Name} ({selected.Size})");
                    _nav.GetAction();

                    resetModal();
                }

                // Back
                if (state.SelectedModalButton == 2)
                    resetModal();

                break;
                
            case NavigationAction.Back:
                resetModal();
                break;

            case NavigationAction.Up:
                if (state.SelectedModalButton == 0 && state.SelectedSizeIndex > 0)
                    state.SelectedSizeIndex--;
                else if (state.SelectedModalButton > 0)
                    state.SelectedModalButton--;
                break;

            case NavigationAction.Down:
                if (state.SelectedModalButton == 0 && state.SelectedSizeIndex < modalVariants.Count - 1)
                    state.SelectedSizeIndex++;
                else if (state.SelectedModalButton < 2)
                    state.SelectedModalButton++;
                break;

            case NavigationAction.Right:
                if (state.SelectedModalButton == 0)
                    state.SelectedModalButton = 1;
                break;

            case NavigationAction.Left:
                if (state.SelectedModalButton > 0)
                    state.SelectedModalButton = 0;
                break;
        }
    }
}