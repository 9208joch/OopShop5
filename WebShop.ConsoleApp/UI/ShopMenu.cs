using System;
using System.Linq;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using UI;
using WebShop.ConsoleApp;
using WebShop.ConsoleApp.UI.Navigation;
using WebShop.ConsoleApp.UI.Rendering;
using WebShop.ConsoleApp.UI.State;
using static System.Collections.Specialized.BitVector32;

public class ShopMenu
{
    private readonly ShopState _state = new();
    private readonly IProductRepository _repo;
    private readonly ConsoleNavigationService _nav;
    private DateTime lastTypingTime = DateTime.MinValue;
    private List<Product> modalVariants = new();
    private List<Product> searchResults = new();

    private string[] sidebarOptions = { "Sale", "Categories", "Home Page", "Shopping Cart","Search" };
    private string[] categories = { "Sweater", "Shorts", "T-shirt", "Jeans", "Jacket" };

    private bool sizeErrorShown = false;
    private DateTime lastErrorTime = DateTime.MinValue;
    private readonly ShoppingCartMenu _shoppingCartMenu;
    private readonly CartService _cartService;

    private readonly ShopRenderer _renderer = new();
    private readonly ShopNavigationHandler _navHandler;
    private readonly ShopModalNavigationHandler _modalHandler;
    private readonly ShopSidebarNavigationHandler _sidebarHandler;
    private readonly ShopSearchHandler _searchHandler;
    private readonly CommonRenderer _commonRenderer = new();

    public ShopMenu(
    IProductRepository repo,
    ConsoleNavigationService nav,
    CartService cartService,
    ShoppingCartMenu shoppingCartMenu)
    {
        _repo = repo;
        _nav = nav;
        _cartService = cartService;
        _shoppingCartMenu = shoppingCartMenu;

        _navHandler = new ShopNavigationHandler(_repo, _cartService);
        _modalHandler = new ShopModalNavigationHandler(_cartService, _nav);
        _sidebarHandler = new ShopSidebarNavigationHandler();
        _searchHandler = new ShopSearchHandler(_repo);
    }
    private async Task LoadOffers()
    {
        _state.Offers = (await _repo.GetAllAsync())
            .Where(p => p.IsOnSale && p.Inventory > 0)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => Guid.NewGuid())
            .ToList();

        _state.SelectedOfferIndex = 0;
    }
    private async Task LoadProducts()
    {
        var rnd = new Random();

        _state.Products = (await _repo.GetAllAsync())
            .Where(p => !p.IsOnSale)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => rnd.Next())
            .Take(36)
            .ToList();

        _state.SelectedProductIndex = 0;
    }

    private async Task LoadCategoryProducts(string category)
    {
        var all = await _repo.GetAllAsync();

        _state.FilteredProducts = all
            .Where(p => p.Category == category)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .ToList();

        _state.SelectedProductIndex = 0;
    }
 
    private void ResetModal()
    {
        _state.InModal = false;
        _state.SelectedSizeIndex = 0;
        _state.SelectedModalButton = 0;
        sizeErrorShown = false;
    }
    
    private void TriggerSizeError()
    {
        sizeErrorShown = true;
        lastErrorTime = DateTime.Now;
    }

    private void HandleOffersNavigation(NavigationAction action)
    {
        var list = _state.SaleActive ? _state.Offers : _state.Offers.Take(6).ToList();

        int cols = 3;

        int startY = 6;
        int visibleRows = (Console.WindowHeight - startY) / 7;
        int itemsPerPage = visibleRows * cols;

        switch (action)
        {
            case NavigationAction.Left:
                if (_state.SelectedOfferIndex % cols > 0)
                    _state.SelectedOfferIndex--;
                else
                {
                    _state.InOffers = false;
                    _state.InSidebar = true;
                }
                break;

            case NavigationAction.Right:
                if (_state.SelectedOfferIndex % cols < cols - 1 &&
                    _state.SelectedOfferIndex < list.Count - 1)
                    _state.SelectedOfferIndex++;
                break;

            case NavigationAction.Up:

                if (_state.SelectedOfferIndex - cols >= 0)
                    _state.SelectedOfferIndex -= cols;

                if (_state.SelectedOfferIndex < _state.OfferScrollOffset)
                    _state.OfferScrollOffset -= cols;

                if (_state.OfferScrollOffset < 0)
                    _state.OfferScrollOffset = 0;
                
                break;

            case NavigationAction.Down:

                if (_state.SelectedOfferIndex + cols < list.Count)
                {
                    _state.SelectedOfferIndex += cols;
                }
                else
                {
                    //  Endast i HOME (inte i sale)
                    if (!_state.SaleActive)
                    {
                        _state.InOffers = false;
                        _state.InProducts = true;
                        _state.SelectedProductIndex = 0;
                    }
                }

                //  Scroll (endast i sale)
                if (_state.SaleActive)
                {
                    if (_state.SelectedOfferIndex >= _state.OfferScrollOffset + itemsPerPage)
                        _state.OfferScrollOffset += cols;
                }
                break;

            case NavigationAction.Select:
                var product = list[_state.SelectedOfferIndex];

                _cartService.AddToCart(product, 1);

                Console.Clear();
                Console.WriteLine($"Added {product.Name} to cart");
                _nav.GetAction();
                break;
        }
    }
    public async Task Start()
    {
        Console.CursorVisible = false;

        await LoadOffers();
        await LoadProducts();

        // Initial state
        _state.InOffers = true;
        _state.InProducts = false;
        _state.InSidebar = false;
        _state.InModal = false;
        _state.CategoriesOpen = false;
        _state.CategoryActive = false;
        _state.SaleActive = false;
        _state.SearchActive = false;
        _state.SearchTyping = false;

        bool running = true;

        while (running)
        {
            //  SEARCH TYPING MODE
            if (_state.SearchTyping)
            {
                Console.SetCursorPosition(0, 0);

                _commonRenderer.DrawHeader();
                _renderer.DrawSidebar(_state, sidebarOptions, categories);
                _renderer.DrawSearchBox(_state);

                await _searchHandler.HandleInput(
                    _state,
                    searchResults
                    );

                await Task.Delay(16);
                continue;
            }

            //  NORMAL RENDER
            Console.Clear();

            _commonRenderer.DrawHeader();
            _renderer.DrawSidebar(_state, sidebarOptions, categories);

            if (_state.SearchActive)
            {
                _renderer.DrawSearchBox(_state);

                if (!_state.SearchTyping)
                    _renderer.DrawProducts(_state, searchResults);
            }
            else if (_state.SaleActive)
            {
                _renderer.DrawOffers(_state);
            }
            else if (!_state.CategoryActive)
            {
                _renderer.DrawOffers(_state);
                _renderer.DrawProducts(_state, searchResults);
            }
            else
            {
                _renderer.DrawProducts(_state, searchResults);
            }

            //  MODAL
            if (_state.InModal)
            {
                _renderer.DrawModal(_state, modalVariants, sizeErrorShown, lastErrorTime);
            }

            // INPUT
            var action = _nav.GetAction();

            //  SEARCH INPUT
            if (_state.SearchActive)
            {
                await _searchHandler.HandleInput(
                    _state,
                    searchResults);
            }
            //  GLOBAL BACK
            if (action == NavigationAction.Back)
            {
                _state.CategoriesOpen = false;

                if (_state.InModal)
                {
                    _state.InModal = false;
                    continue;
                }
                if (_state.InProducts || _state.InOffers || _state.SearchActive)
                {
                    _state.InSidebar = true;
                    _state.InProducts = false;
                    _state.InOffers = false;
                    _state.SearchActive = false;
                    _state.SaleActive = false;
                    continue;
                }
                running = false;
                break;
            }
            //  STATE ROUTING
            if (_state.InModal)
            {
                await _modalHandler.Handle(
                    _state,
                    action,
                    modalVariants,
                    ResetModal,
                    TriggerSizeError);
            }
            else if (_state.InSidebar)
            {
                await _sidebarHandler.Handle(
                    _state,
                    action,
                    categories,
                    LoadCategoryProducts,
                    LoadOffers,
                    async () => await _shoppingCartMenu.Start()
                );
                // HOME → exit
                if (!_state.InSidebar &&
                    !_state.InOffers &&
                    !_state.InProducts &&
                    !_state.InModal &&
                    !_state.SearchActive)
                {
                    running = false;
                    break;
                }
            }
            else if (_state.InOffers)
            {
                HandleOffersNavigation(action);
            }
            else if (_state.InProducts)
            {
                await _navHandler.HandleProductsNavigation(
                    _state,
                    action,
                    searchResults,
                    modalVariants);
            }
            await Task.Delay(16);
        }
        Console.Clear();
    }
}

