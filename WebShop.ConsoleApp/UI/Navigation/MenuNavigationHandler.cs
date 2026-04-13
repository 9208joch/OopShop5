using System;
using System.Collections.Generic;
using System.Text;
using WebShop.ConsoleApp.UI.State;

namespace WebShop.ConsoleApp.UI.Navigation
{
    public class MenuNavigationHandler
    {
        public void HandleOffers(MenuState state, NavigationAction action, int count)
        {
            switch (action)
            {
                case NavigationAction.Left:
                    state.SelectedOfferIndex = (state.SelectedOfferIndex - 1 + count) % count;
                    break;

                case NavigationAction.Right:
                    state.SelectedOfferIndex = (state.SelectedOfferIndex + 1) % count;
                    break;

                case NavigationAction.Down:
                    state.InOffers = false;
                    state.SelectedIndex = 0;
                    break;
            }
        }

        public void HandleMenu(MenuState state, NavigationAction action, int optionsLength)
        {
            switch (action)
            {
                case NavigationAction.Up:
                    if (state.SelectedIndex == 0)
                        state.InOffers = true;
                    else
                        state.SelectedIndex--;
                    break;

                case NavigationAction.Down:
                    state.SelectedIndex = (state.SelectedIndex + 1) % optionsLength;
                    break;

                case NavigationAction.Left:
                    state.InOffers = true;
                    break;
            }
        }
    }
}
