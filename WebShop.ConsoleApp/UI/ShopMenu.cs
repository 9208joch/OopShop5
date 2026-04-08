using System;
using System.Linq;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;
using _2.WebShop.Application.Services;
using UI;
using WebShop.ConsoleApp;
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
    }


 
    private async Task LoadOffers()
    {
        _state.Offers = (await _repo.GetAllAsync())
            .Where(p => p.IsOnSale && p.Inventory > 0)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => Guid.NewGuid())
            //.Take(6) <-- test
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
    
    private void DrawSidebar()
    {
        int x = 2;
        int y = 5;

        for (int i = 0; i < sidebarOptions.Length; i++)
        {
            Console.SetCursorPosition(x, y);

            bool selected = _state.InSidebar && i == _state.SelectedSidebarIndex;

            //  Highlight för vald rad
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ResetColor();
            }

            //  SALE (grön text)
            if (i == 0)
            {
                if (!selected)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("Sale");
            }
            //  CATEGORIES med pil
            else if (i == 1)
            {
                string arrow = _state.CategoriesOpen ? "▲" : "▼";
                Console.WriteLine($"Categories {arrow}");
            }
            //  Övriga
            else
            {
                Console.WriteLine(sidebarOptions[i]);
            }

            Console.ResetColor();
            y++;

  
            // CATEGORY DROPDOWN
           
            if (i == 1 && _state.CategoriesOpen)
            {
                for (int j = 0; j < categories.Length; j++)
                {
                    Console.SetCursorPosition(x + 2, y);

                    bool catSelected =
                        _state.InSidebar &&
                        _state.SelectedSidebarIndex == 1 &&
                        _state.SelectedCategoryIndex == j;

                    if (catSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ResetColor();
                    }

                    Console.WriteLine(categories[j]);

                    Console.ResetColor();
                    y++;
                }
            }
        }
    }

    private void DrawOffers()
    {
        int cardWidth = 30;
        int innerWidth = 28;

        int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;
        int startY = 6;

        string title = _state.SaleActive ? "=== Sale ===" : "=== Great _state.Offers ===";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, startY - 2);
        Console.WriteLine(title);

        var list = _state.SaleActive ? _state.Offers : _state.Offers.Take(6).ToList();

        int visibleRows = (Console.WindowHeight - startY) / 7;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + _state.OfferScrollOffset;
            if (index >= list.Count) break;

            var p = list[index];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 7;

            bool isSelected = index == _state.SelectedOfferIndex && _state.InOffers;

            if (isSelected)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(x, y);
            Console.WriteLine($"{p.Name} ({p.Size})".PadRight(innerWidth));

            var lines = WrapText(p.Description ?? "", innerWidth).Take(2).ToList();

            string line1 = lines.ElementAtOrDefault(0) ?? "";
            string line2 = lines.ElementAtOrDefault(1) ?? "";

            Console.SetCursorPosition(x, y + 1);
            Console.WriteLine(line1.PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine(line2.PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 3);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Sale: {p.SalePrice ?? p.Price} kr".PadRight(innerWidth));

            Console.ResetColor();

            Console.SetCursorPosition(x, y + 5);

            if (isSelected)
                Highlight();

            Console.WriteLine("[Add to cart]".PadRight(innerWidth));
            Console.ResetColor();
        }

        //  Scroll indicators
        bool canScrollDown = _state.OfferScrollOffset + itemsPerPage < list.Count;
        bool canScrollUp = _state.OfferScrollOffset > 0;

        if (_state.SaleActive && canScrollDown)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, Console.WindowHeight - 1);
            Console.Write("[More ↓]");
        }

        if (_state.SaleActive && canScrollUp)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, startY - 1);
            Console.Write("[↑ More]");
        }
    }
    private List<string> WrapText(string text, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var current = "";

        foreach (var word in words)
        {
            if ((current + word).Length > maxWidth)
            {
                lines.Add(current.Trim());
                current = "";
            }

            current += word + " ";
        }

        if (!string.IsNullOrWhiteSpace(current))
            lines.Add(current.Trim());
        
        return lines;
    }

    private void DrawProducts()
    {
        int cardWidth = 30;
        int innerWidth = 28;

        int startY = _state.CategoryActive ? 6 : 20;
        int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;
        int maxHeight = Console.WindowHeight - 2;

    
        var list = _state.SearchActive
            ? searchResults
            : (_state.CategoryActive ? _state.FilteredProducts : _state.Products);

        if (list.Count == 0)
        {
            Console.SetCursorPosition(startX, startY);
            Console.WriteLine("No _state.InProducts found...");
            return;
        }

        int visibleRows = (Console.WindowHeight - startY) / 4;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + _state.ProductScrollOffset;
            if (index >= list.Count) break;

            var p = list[index];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 4;

            bool isSelected = index == _state.SelectedProductIndex && _state.InProducts;

            //  highlight hela kortet
            if (isSelected)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            //  NAME
            Console.SetCursorPosition(x, y);
            DrawHighlightedText(p.Name, _state.SearchQuery, x, y, innerWidth);

            //  PRICE (sale stöd)
            Console.SetCursorPosition(x, y + 1);

            if (p.IsOnSale)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"Old: {p.Price} ");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"New: {p.SalePrice}".PadRight(innerWidth));
            }
            else
            {
                Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));
            }

            Console.ResetColor();

            //  BUTTON
            Console.SetCursorPosition(x, y + 2);

            if (isSelected)
                Highlight();

            Console.WriteLine("[More info]".PadRight(innerWidth));
            Console.ResetColor();
        }

        //  SCROLL indikatorer
        bool canScrollDown = _state.ProductScrollOffset + itemsPerPage < list.Count;
        bool canScrollUp = _state.ProductScrollOffset > 0;

        if (canScrollDown)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, maxHeight - 1);
            Console.Write("[More ↓]");
        }

        if (canScrollUp)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, startY - 1);
            Console.Write("[↑ More]");
        }
    }
    private void DrawHighlightedText(string text, string query, int x, int y, int width)
    {
        Console.SetCursorPosition(x, y);

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine(text.PadRight(width));
            return;
        }

        int index = text.IndexOf(query, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
        {
            Console.WriteLine(text.PadRight(width));
            return;
        }

        // före match
        Console.Write(text.Substring(0, index));

        //  highlight match
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(text.Substring(index, query.Length));
        Console.ResetColor();

        // efter match
        Console.Write(text.Substring(index + query.Length));

        // fyll resten
        int remaining = width - text.Length;
        if (remaining > 0)
            Console.Write(new string(' ', remaining));
    }
    private void DrawSearchBox()
    {
        int width = 50;
        int x = (Console.WindowWidth - width) / 2;
        int y = 5;

        //  Titel
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Search _state.InProducts:");

        //  Input box
        Console.SetCursorPosition(x, y + 1);
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.White;

        string displayText = _state.SearchQuery;

        //  Klipp text om den för lång
        if (displayText.Length > width)
            displayText = displayText[^width..];

        Console.Write(displayText.PadRight(width));

        Console.ResetColor();

        //  Sätt cursor i slutet av texten 
        int cursorPos = Math.Min(displayText.Length, width - 1);
        Console.SetCursorPosition(x + cursorPos, y + 1);

        //  Visa cursor endast när skriver
        Console.CursorVisible = _state.SearchTyping;
    }
    private void DrawSearchResults()
    {
        int cardWidth = 30;
        int innerWidth = 28;

        int startY = 10;
        int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;

        int maxHeight = Console.WindowHeight - 2;

        int visibleRows = (maxHeight - startY) / 4;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + _state.ProductScrollOffset;
            if (index >= searchResults.Count) break;

            var p = searchResults[index];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 4;

            Console.SetCursorPosition(x, y);
            Console.WriteLine(p.Name.PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 1);

            if (p.IsOnSale)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"Old: {p.Price} ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"New: {p.SalePrice}");
            }
            else
            {
                Console.WriteLine($"Price: {p.Price}");
            }

            Console.ResetColor();

            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine("[More info]");
        }
    }
    
    private async Task HandleSearchInput(NavigationAction action)
    {

        //  INPUT 
        
        if (_state.SearchTyping && Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);

            //  ESC  lämna search direkt
            if (key.Key == ConsoleKey.Escape)
            {
                _state.SearchActive = false;
                _state.SearchTyping = false;

                _state.SearchQuery = "";
                searchResults.Clear();

                _state.SelectedProductIndex = 0;
                _state.ProductScrollOffset = 0;

                _state.InSidebar = true;
                _state.InProducts = false;
                _state.InOffers = false;

                return;
            }
            
            //  BACKSPACE
            if (key.Key == ConsoleKey.Backspace && _state.SearchQuery.Length > 0)
            {
                _state.SearchQuery = _state.SearchQuery[..^1];
            }
            //  ENTER  trigga direkt sökning
            else if (key.Key == ConsoleKey.Enter && _state.SearchQuery.Length >= 2)
            {
                await ExecuteSearch();
                return;
            }
            //  TEXT INPUT
            else if (!char.IsControl(key.KeyChar))
            {
                _state.SearchQuery += key.KeyChar;
            }

            // reset timer
            lastTypingTime = DateTime.Now;
        }
        
   
        //  AUTO SEARCH (1 sek delay)
        
        if (_state.SearchTyping &&
            _state.SearchQuery.Length >= 2 &&
            (DateTime.Now - lastTypingTime).TotalSeconds >= 1)
        {
            await ExecuteSearch();
        }
    }
    
    private async Task ExecuteSearch()
    {
        var all = await _repo.GetAllAsync();

        searchResults = all
            .Where(p => p.Name.Contains(_state.SearchQuery, StringComparison.OrdinalIgnoreCase))
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .ToList();

        //  växla till resultatläge
        _state.SearchTyping = false;

        _state.SelectedProductIndex = 0;
        _state.ProductScrollOffset = 0;

        _state.InProducts = true;
        _state.InOffers = false;
        _state.InSidebar = false;
    }

    private void DrawModal()
    {
        int width = 70;
        int height = Math.Min(25, 12 + modalVariants.Count);

        int x = (Console.WindowWidth - width) / 2;
        int y = 3;

        //  Bakgrund
        Console.BackgroundColor = ConsoleColor.DarkGray;

        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(new string(' ', width));
        }

        Console.ResetColor();

        //  Helper för att alltid sätta rätt bakgrund
        void WriteLineInModal(int posX, int posY, string text)
        {
            Console.SetCursorPosition(posX, posY);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(text.PadRight(width - 4));
            Console.ResetColor();
        }

        //  Titel
        WriteLineInModal(x + 2, y + 1, _state.ModalProduct!.Name);

        //  Beskrivning
        WriteLineInModal(x + 2, y + 3, _state.ModalProduct.Description);

        //  Pris
        WriteLineInModal(x + 2, y + 5, $"Price: {_state.ModalProduct.Price} kr");

        //  Label
        WriteLineInModal(x + 2, y + 7, "Sizes:");

     
        //  SIZES 
       
        for (int i = 0; i < modalVariants.Count; i++)
        {
            int posY = y + 8 + i;
            if (posY >= y + height - 2) break;

            var v = modalVariants[i];

            Console.SetCursorPosition(x + 4, posY);

            string text = $"{v.Size,-10} ({v.Inventory,2})";

            //  default
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            //  out of stock
            if (v.Inventory == 0)
                Console.ForegroundColor = ConsoleColor.Red;

            //  SELECTED (tryckt enter)
            if (_state.SelectedSize == i)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            //  HOVER (cursor)
            if (_state.SelectedModalButton == 0 && i == _state.SelectedSizeIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine(text.PadRight(20));
            Console.ResetColor();
        }

    
        //  KNAPPAR
        
        int buttonX = x + width - 20;

        // Add to cart
        Console.SetCursorPosition(buttonX, y + 10);
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.Black;


        if (_state.SelectedModalButton == 1)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
        }

        Console.WriteLine("[Add to cart]");
        Console.ResetColor();

        // Back
        Console.SetCursorPosition(buttonX, y + 12);
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.Black;

        if (_state.SelectedModalButton == 2)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
        }

        Console.WriteLine("[Back]");
        Console.ResetColor();

        
        // ERROR
        
        if (sizeErrorShown && (DateTime.Now - lastErrorTime).TotalSeconds < 2)
        {
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("Please select a size!".PadRight(width - 4));

            Console.ResetColor();
        }
    }
    private void Highlight()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
    }




    private async Task HandleProductsNavigation(NavigationAction action)
    {
        var list = _state.SearchActive
            ? searchResults
            : (_state.CategoryActive ? _state.FilteredProducts : _state.Products);

        if (list.Count == 0)
            return;

        int cols = 3;

        switch (action)
        {
            case NavigationAction.Left:
                _state.InSidebar = true;
                _state.InProducts = false;
                break;
                
            case NavigationAction.Right:
                if (_state.SelectedProductIndex % cols < cols - 1 &&
                    _state.SelectedProductIndex < list.Count - 1)
                {
                    _state.SelectedProductIndex++;
                }
                break;
                
            case NavigationAction.Up:

                //  gå upp till _state.Offers
                if (!_state.CategoryActive && !_state.SaleActive &&
                    _state.SelectedProductIndex < cols &&
                    _state.ProductScrollOffset == 0)
                {
                    _state.InOffers = true;
                    _state.SelectedOfferIndex = 0;
                    return;
                }

                if (_state.SelectedProductIndex - cols >= 0)
                    _state.SelectedProductIndex -= cols;

                if (_state.SelectedProductIndex < _state.ProductScrollOffset)
                    _state.ProductScrollOffset -= cols;

                if (_state.ProductScrollOffset < 0)
                    _state.ProductScrollOffset = 0;
                break;

            case NavigationAction.Down:

                if (_state.SelectedProductIndex + cols < list.Count)
                    _state.SelectedProductIndex += cols;

                int startY = _state.CategoryActive ? 6 : 20;
                int visibleRows = (Console.WindowHeight - startY) / 4;
                int maxVisibleItems = visibleRows * cols;

                if (_state.SelectedProductIndex >= _state.ProductScrollOffset + maxVisibleItems)
                    _state.ProductScrollOffset += cols;

                break;

            case NavigationAction.Select:

                if (_state.SelectedProductIndex < 0 || _state.SelectedProductIndex >= list.Count)
                    return;

                _state.ModalProduct = list[_state.SelectedProductIndex];

                modalVariants = (await _repo.GetAllAsync())
                    .Where(p => p.Name == _state.ModalProduct.Name)
                    .OrderBy(p => p.Size)
                    .ToList();

                _state.SelectedSizeIndex = 0;
                _state.SelectedModalButton = 0;
                sizeErrorShown = false;

                _state.SelectedSize = null; 

                _state.InModal = true;
                break;
        }
    }

    private async Task HandleModalNavigation(NavigationAction action)
    {
        switch (action)
        {
            case NavigationAction.Select:

                // välj storlek
                if (_state.SelectedModalButton == 0)
                {
                    _state.SelectedSize = _state.SelectedSizeIndex;
                    return;
                }

                // Add to cart
                if (_state.SelectedModalButton == 1)
                {
                    if (_state.SelectedSize == null)
                    {
                        TriggerSizeError();
                        return;
                    }

                    var selected = modalVariants[_state.SelectedSize.Value];

                    if (selected.Inventory == 0)
                    {
                        TriggerSizeError();
                        return;
                    }

                    _cartService.AddToCart(selected, 1);

                    Console.Clear();
                    Console.WriteLine($"Added {selected.Name} ({selected.Size})");
                    _nav.GetAction();

                    ResetModal();
                }

                // Back-knappen
                if (_state.SelectedModalButton == 2)
                    ResetModal();

                break;

            case NavigationAction.Back:
                ResetModal();
                break;

            case NavigationAction.Up:
                if (_state.SelectedModalButton == 0 && _state.SelectedSizeIndex > 0)
                    _state.SelectedSizeIndex--;
                else if (_state.SelectedModalButton > 0)
                    _state.SelectedModalButton--;
                break;

            case NavigationAction.Down:
                if (_state.SelectedModalButton == 0 && _state.SelectedSizeIndex < modalVariants.Count - 1)
                    _state.SelectedSizeIndex++;
                else if (_state.SelectedModalButton < 2)
                    _state.SelectedModalButton++;
                break;

            case NavigationAction.Right:
                if (_state.SelectedModalButton == 0)
                    _state.SelectedModalButton = 1;
                break;

            case NavigationAction.Left:
                if (_state.SelectedModalButton > 0)
                    _state.SelectedModalButton = 0;
                break;
        }
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
    private async Task HandleSidebarNavigation(NavigationAction action)
    {
        
  
        // DROPDOWN AKTIV
        
        if (_state.CategoriesOpen && _state.SelectedSidebarIndex == 1)
        {
            switch (action)
            {
                case NavigationAction.Up:
                    if (_state.SelectedCategoryIndex > 0)
                        _state.SelectedCategoryIndex--;
                    break;

                case NavigationAction.Down:
                    if (_state.SelectedCategoryIndex < categories.Length - 1)
                        _state.SelectedCategoryIndex++;
                    break;

                case NavigationAction.Select:
                    _state.SelectedCategory = categories[_state.SelectedCategoryIndex];
                    _state.CategoryActive = true;

                    await LoadCategoryProducts(_state.SelectedCategory);

                    _state.CategoriesOpen = false;     // stäng dropdown
                    _state.InSidebar = false;
                    _state.InProducts = true;
                    _state.InOffers = false;

                    _state.ProductScrollOffset = 0;
                    _state.SelectedProductIndex = 0;
                    break;

                case NavigationAction.Back:
                case NavigationAction.Left:
                    _state.CategoriesOpen = false;     // stäng dropdown
                    break;
            }

            return;
        }

        // VANLIG SIDEBAR
        
        switch (action)
        {
            case NavigationAction.Up:
                if (_state.SelectedSidebarIndex > 0)
                    _state.SelectedSidebarIndex--;
                break;

            case NavigationAction.Down:
                if (_state.SelectedSidebarIndex < sidebarOptions.Length - 1)
                    _state.SelectedSidebarIndex++;
                break;

            case NavigationAction.Right:
                _state.InSidebar = false;
                _state.InOffers = !_state.CategoryActive;
                _state.InProducts = _state.CategoryActive;
                break;

            case NavigationAction.Select:
                
                
                //  SALE
                
                if (_state.SelectedSidebarIndex == 0)
                {
                    _state.SaleActive = true;
                    _state.CategoryActive = false;

                    _state.OfferScrollOffset = 0; 
                    _state.SelectedOfferIndex = 0;

                    _state.InSidebar = false;
                    _state.InOffers = true;
                    _state.InProducts = false;

                    await LoadOffers();
                    return;
                }

                
                //  CATEGORIES
                
                if (_state.SelectedSidebarIndex == 1)
                {
                    _state.CategoriesOpen = !_state.CategoriesOpen;
                    _state.SelectedCategoryIndex = 0;
                    return;
                }

                
                // HOME PAGE
                
                if (_state.SelectedSidebarIndex == 2)
                {
                    //  reset ALL state
                    _state.SaleActive = false;
                    _state.CategoryActive = false;
                    _state.CategoriesOpen = false;

                    _state.InOffers = false;
                    _state.InProducts = false;
                    _state.InSidebar = false;
                    _state.InModal = false;

                    return; // lämnar Start()
                }


                // SHOPPING CART

                if (_state.SelectedSidebarIndex == 3)
                {
                    await _shoppingCartMenu.Start();
                    return;
                }
                //  SEARCH
                if (_state.SelectedSidebarIndex == 4)
                {
                    //  Stäng ALLA andra lägen
                    _state.SaleActive = false;
                    _state.CategoryActive = false;
                    _state.CategoriesOpen = false;

                    _state.InModal = false;

                    //  Aktivera search
                    _state.SearchActive = true;
                    _state.SearchTyping = true;

                    //  Reset input & resultat
                    _state.SearchQuery = "";
                    searchResults.Clear();

                    //  Reset navigation
                    _state.SelectedProductIndex = 0;
                    _state.ProductScrollOffset = 0;

                    //  Lämna andra vyer
                    _state.InSidebar = false;
                    _state.InOffers = false;
                    _state.InProducts = false;

                    return;
                }
                
                break;
        }
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
            // SEARCH TYPING MODE
            if (_state.SearchTyping)
            {
                Console.SetCursorPosition(0, 0);

                DrawHeader();
                DrawSidebar();
                DrawSearchBox();

                await HandleSearchInput(NavigationAction.None);

                await Task.Delay(16);
                continue;
            }

            // NORMAL RENDER
            Console.Clear();

            DrawHeader();
            DrawSidebar();

            if (_state.SearchActive)
            {
                DrawSearchBox();

                if (!_state.SearchTyping)
                    DrawProducts();
            }
            else if (_state.SaleActive)
            {
                DrawOffers();
            }
            else if (!_state.CategoryActive)
            {
                DrawOffers();
                DrawProducts();
            }
            else
            {
                DrawProducts();
            }

            // MODAL
            if (_state.InModal)
                DrawModal();

            // INPUT
            var action = _nav.GetAction();

            // SEARCH INPUT
            if (_state.SearchActive)
                await HandleSearchInput(action);

            // GLOBAL BACK
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

            // STATE ROUTING
            if (_state.InModal)
            {
                await HandleModalNavigation(action);
            }
            else if (_state.InSidebar)
            {
                await HandleSidebarNavigation(action);

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
                await HandleProductsNavigation(action);
            }

            await Task.Delay(16);
        }

        Console.Clear();
    }

    private void DrawHeader()
    {
        string title = "Backends Clothing";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 1);
        Console.WriteLine(title);
    }
}

