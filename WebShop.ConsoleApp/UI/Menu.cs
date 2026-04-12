using System;
using System.Linq;
using _2.WebShop.Application;           
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using _4.WebShop.ConsoleApp.UI;
using UI;
using WebShop.ConsoleApp.UI.Navigation;
using WebShop.ConsoleApp.UI.Rendering;
using WebShop.ConsoleApp.UI.State;


namespace WebShop.ConsoleApp.UI;
public class Menu
{
    private readonly ShopMenu _shop;
    private readonly IDistanceService _distanceService;
    private readonly AdminMenu _adminMenu;
    private readonly MenuState _state = new();
    private readonly MenuRenderer _renderer = new();
    private readonly MenuNavigationHandler _navHandler = new();
    private readonly CommonRenderer _commonRenderer = new();
    private readonly CartService _cartService;
    private readonly ShoppingCartMenu _shoppingCartMenu;   //NK...
    private double? _cachedDistance = null;


    public Menu(
    IProductRepository repo,
    ConsoleNavigationService nav,
    ShopMenu shop,
    ShoppingCartMenu shoppingCartMenu,
    AdminMenu adminMenu,
    CartService cartService,
    IDistanceService distanceService)  
    {
        _repo = repo;
        _nav = nav;
        _shop = shop;
        _shoppingCartMenu = shoppingCartMenu;
        _adminMenu = adminMenu;
        _cartService = cartService;
        _distanceService = distanceService; 
    }
    private readonly IProductRepository _repo;

    private readonly ConsoleNavigationService _nav;
   
    private string[] options = new[]
    {
        "Home Page",
        "Browse Products",
        "Shopping Cart",
        "Admin",
        "Quit"
    };
    private async Task DrawStoreInfo()
    {
        string address = "Kungsgatan 4 451 30 Uddevalla";
        int center = Console.WindowWidth / 2;

        //  Adress
        Console.SetCursorPosition(center - address.Length / 2, 4);
        Console.WriteLine(address);

        if (_distanceService == null)
            return;

        try
        {
            //  Hämta endast en gång (cache)
            if (_cachedDistance == null)
            {
                string customerAddress = "Stockholm"; // placeholder
                _cachedDistance = await _distanceService.GetDistanceToStoreAsync(customerAddress);
            }

            string text = _cachedDistance != null
                ? $"Distance to store: {Math.Round(_cachedDistance.Value, 1)} km"
                : "Distance unavailable";

            Console.SetCursorPosition(center - text.Length / 2, 5);
            Console.WriteLine(text);
        }
        catch
        {
            string error = "Distance unavailable";

            Console.SetCursorPosition(center - error.Length / 2, 5);
            Console.WriteLine(error);
        }
    }
    public async Task Start()
    {
        Console.CursorVisible = false;

        _state.Offers = await GetOffers();

        if (!_state.Offers.Any())
        {
            Console.WriteLine("No offers available.");
            _nav.GetAction();
            return;
        }
        
        while (true)
        {
            Console.Clear();

            _commonRenderer.DrawHeader();
            await DrawStoreInfo();
            _renderer.DrawOffers(_state);
            _renderer.DrawMenu(_state, options);
            
            
            var action = _nav.GetAction();

            if (_state.InOffers)
            {
                _navHandler.HandleOffers(_state, action, _state.Offers.Count);

                if (action == NavigationAction.Select)
                {
                    var product = _state.Offers[_state.SelectedOfferIndex];

                    _cartService.AddToCart(product, 1);

                    Console.Clear();
                    Console.WriteLine($"Added {product.Name} to cart!");
                    _nav.GetAction();
                }

                if (action == NavigationAction.Back)
                    return;
            }
            else
            {
                _navHandler.HandleMenu(_state, action, options.Length);

                if (action == NavigationAction.Select)
                {
                    bool reload = await HandleSelection();

                    if (reload)
                    {
                        _state.Offers = await GetOffers();
                        _state.InOffers = true;
                        _state.SelectedOfferIndex = 0;
                    }
                }

                if (action == NavigationAction.Back)
                    return;
            }
        }
    }
    private async Task<List<Product>> GetOffers()
    {
        var products = await _repo.GetAllAsync();

        return products
            .Where(p => p.IsOnSale && p.Inventory > 0)
            .OrderBy(x => Guid.NewGuid())
            .Take(3)
            .ToList();
    }
    private async Task<bool> HandleSelection()
    {
        switch (_state.SelectedIndex)
        {
            case 0:
                return true;

            case 1:
                if (_shop != null)
                {
                    await _shop.Start();
                }
                break;

            case 2:
                Console.Clear();
                if (_shoppingCartMenu !=null)
                {

                    await _shoppingCartMenu.Start();   //NK...
                }
                break;
                
            case 3:
                Console.Clear();
                if (_adminMenu != null)
                {

                    await _adminMenu.ShowMenuAsync();
                }
                break;

            case 4:
                Environment.Exit(0);
                break;
        }
        return false;
    }
}