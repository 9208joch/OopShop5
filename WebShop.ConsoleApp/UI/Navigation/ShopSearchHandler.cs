using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using WebShop.ConsoleApp.UI.State;

public class ShopSearchHandler
{
    private readonly IProductRepository _repo;

    public ShopSearchHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task HandleInput(
        ShopState state,
        List<Product> searchResults)
    {
        if (state.SearchTyping && Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);

            // ESC
            if (key.Key == ConsoleKey.Escape)
            {
                state.SearchActive = false;
                state.SearchTyping = false;

                state.SearchQuery = "";
                searchResults.Clear();

                state.SelectedProductIndex = 0;
                state.ProductScrollOffset = 0;

                state.InSidebar = true;
                state.InProducts = false;
                state.InOffers = false;

                return;
            }

            // BACKSPACE
            if (key.Key == ConsoleKey.Backspace && state.SearchQuery.Length > 0)
            {
                state.SearchQuery = state.SearchQuery[..^1];
            }
            // ENTER
            else if (key.Key == ConsoleKey.Enter && state.SearchQuery.Length >= 2)
            {
                await ExecuteSearch(state, searchResults);
                return;
            }
            // TEXT
            else if (!char.IsControl(key.KeyChar))
            {
                state.SearchQuery += key.KeyChar;
            }

            state.LastTypingTime = DateTime.Now;
        }

        // AUTO SEARCH (1 sek)
        if (state.SearchTyping &&
            state.SearchQuery.Length >= 2 &&
            (DateTime.Now - state.LastTypingTime).TotalSeconds >= 1)
        {
            await ExecuteSearch(state, searchResults);
        }
    }

    private async Task ExecuteSearch(
        ShopState state,
        List<Product> searchResults)
    {
        var all = await _repo.GetAllAsync();

        searchResults.Clear();

        searchResults.AddRange(
            all.Where(p => p.Name.Contains(state.SearchQuery, StringComparison.OrdinalIgnoreCase))
               .GroupBy(p => p.Name)
               .Select(g => g.First())
        );

        state.SearchTyping = false;

        state.SelectedProductIndex = 0;
        state.ProductScrollOffset = 0;

        state.InProducts = true;
        state.InOffers = false;
        state.InSidebar = false;
    }
}