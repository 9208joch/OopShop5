using System;
using System.Linq;
using _1.WebShop.Core.Entities;
using _1.WebShop.Core.Interfaces;

public class ShopMenu
{
    private readonly IProductRepository _repo;
    private readonly ConsoleNavigationService _nav;

    private List<Product> offers = new();
    private List<Product> products = new();
    private List<Product> filteredProducts = new();

    private int selectedOfferIndex = 0;
    private int selectedProductIndex = 0;

    private bool inOffers = true;
    private bool inProducts = false;
    private bool inSidebar = false;

    private bool categoriesOpen = false;
    private bool categoryActive = false;

    private int selectedSidebarIndex = 0;
    private int selectedCategoryIndex = 0;

    private string? selectedCategory = null;
    private int productScrollOffset = 0;
    
    

    private string[] sidebarOptions = { "Categories", "Home Page", "Shopping Cart" };
    private string[] categories = { "Sweater", "Shorts", "T-shirt", "Jeans", "Jacket" };


    private bool inModal = false;
    private Product? modalProduct = null;
    private List<Product> modalVariants = new();

    private int selectedSizeIndex = 0;
    private int selectedModalButton = 0;

    private bool sizeErrorShown = false;
    private DateTime lastErrorTime = DateTime.MinValue;

    
    
    public ShopMenu(IProductRepository repo, ConsoleNavigationService nav)
    {
        _repo = repo;
        _nav = nav;
    }



    private async Task LoadOffers()
    {
        offers = (await _repo.GetAllAsync())
            .Where(p => p.IsOnSale && p.Inventory > 0)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => Guid.NewGuid())
            .Take(6)
            .ToList();

        selectedOfferIndex = 0;
    }

    private async Task LoadProducts()
    {
        products = (await _repo.GetAllAsync())
            .Where(p => !p.IsOnSale)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => Guid.NewGuid())
            .Take(36)
            .ToList();

        selectedProductIndex = 0;
    }

    private async Task LoadCategoryProducts(string category)
    {
        var all = await _repo.GetAllAsync();

        filteredProducts = all
            .Where(p => p.Category == category)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .ToList();

        selectedProductIndex = 0;
    }



    private void DrawSidebar()
    {
        int x = 2;
        int y = 5;

        Console.SetCursorPosition(x, y++);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Sale");
        Console.ResetColor();

        y++;

        for (int i = 0; i < sidebarOptions.Length; i++)
        {
            Console.SetCursorPosition(x, y);

            bool selected = inSidebar && i == selectedSidebarIndex;

            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            if (i == 0)
            {
                string arrow = categoriesOpen ? "▲" : "▼";
                Console.WriteLine($"Categories {arrow}");
            }
            else
            {
                Console.WriteLine(sidebarOptions[i]);
            }

            Console.ResetColor();
            y++;

            if (i == 0 && categoriesOpen)
            {
                for (int j = 0; j < categories.Length; j++)
                {
                    Console.SetCursorPosition(x + 2, y);

                    bool catSelected = inSidebar &&
                                       selectedSidebarIndex == 0 &&
                                       selectedCategoryIndex == j;

                    if (catSelected)
                        Console.BackgroundColor = ConsoleColor.DarkGray;

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

        string title = "=== Great Offers ===";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, startY - 2);
        Console.WriteLine(title);

        for (int i = 0; i < offers.Count; i++)
        {
            var p = offers[i];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 6;

            bool selected = i == selectedOfferIndex && inOffers;

            if (selected)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(x, y);
            Console.WriteLine(p.Name.PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 1);
            var lines = WrapText(p.Description, innerWidth).Take(2).ToList();

            Console.SetCursorPosition(x, y + 1);
            Console.WriteLine(lines.ElementAtOrDefault(0)?.PadRight(innerWidth) ?? "");

            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine(lines.ElementAtOrDefault(1)?.PadRight(innerWidth) ?? "");

            Console.SetCursorPosition(x, y + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Sale: {p.SalePrice ?? p.Price} kr".PadRight(innerWidth));

            Console.ResetColor();

            Console.SetCursorPosition(x, y + 4);

            if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }

            Console.WriteLine("[Add to cart]".PadRight(innerWidth));
            Console.ResetColor();
        }
    }
    

    
    private void DrawProducts()
    { 
        int cardWidth = 30;
        int innerWidth = 28;
        
        int startY = categoryActive ? 6 : 20;
        int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;

        int maxHeight = Console.WindowHeight - 2;

        var list = categoryActive ? filteredProducts : products;

        if (categoryActive)
        {
            Console.SetCursorPosition(startX, startY - 2);
            Console.WriteLine($"=== {selectedCategory} ===");
        }

        int visibleRows = (maxHeight - startY) / 4;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + productScrollOffset;

            if (index >= list.Count)
                break;

            var p = list[index];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 4;

            bool isSelected = index == selectedProductIndex && inProducts;

            if (isSelected)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(x, y);
            Console.WriteLine(p.Name.PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));

            Console.ResetColor();

            Console.SetCursorPosition(x, y + 2);

            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }

            Console.WriteLine("[More information]".PadRight(innerWidth));
            Console.ResetColor();
        }
        

        

        bool canScrollDown = productScrollOffset + (itemsPerPage) < list.Count;
        bool canScrollUp = productScrollOffset > 0;

        
        if (canScrollDown)
        {
            Console.SetCursorPosition(Console.WindowWidth - 5, maxHeight - 1);
            Console.Write("↓");
        }

        
        if (canScrollUp)
        {
            Console.SetCursorPosition(Console.WindowWidth - 5, startY - 1);
            Console.Write("↑");
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

    private void DrawModal()
    {
        int width = 60;
        int height = 20;

        int x = (Console.WindowWidth - width) / 2;
        int y = 5;

        Console.BackgroundColor = ConsoleColor.DarkGray;

        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(new string(' ', width));
        }

        Console.ResetColor();

        Console.SetCursorPosition(x + 2, y + 1);
        Console.WriteLine(modalProduct!.Name);

        Console.SetCursorPosition(x + 2, y + 3);
        Console.WriteLine(modalProduct.Description);

        Console.SetCursorPosition(x + 2, y + 5);
        Console.WriteLine($"Price: {modalProduct.Price} kr");

        Console.SetCursorPosition(x + 2, y + 7);
        Console.WriteLine("Sizes:");

        for (int i = 0; i < modalVariants.Count; i++)
        {
            var v = modalVariants[i];

            Console.SetCursorPosition(x + 4, y + 8 + i);

            if (v.Inventory == 0)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            if (selectedModalButton == 0 && i == selectedSizeIndex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine($"{v.Size} ({v.Inventory})");
            Console.ResetColor();
        }

        Console.SetCursorPosition(x + 2, y + 15);
        if (selectedModalButton == 1)
            Highlight();

        Console.WriteLine("[Add to cart]");
        Console.ResetColor();

        Console.SetCursorPosition(x + 2, y + 17);
        if (selectedModalButton == 2)
            Highlight();

        Console.WriteLine("[Back]");
        Console.ResetColor();

        if (sizeErrorShown && (DateTime.Now - lastErrorTime).TotalSeconds < 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x + 2, y + 13);
            Console.WriteLine("Please select a size!");
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
        var list = categoryActive ? filteredProducts : products;

        int cols = 3;

        switch (action)
        {
            case NavigationAction.Left:
                inProducts = false;
                inSidebar = true;
                break;

            case NavigationAction.Right:
                if (selectedProductIndex % cols < cols - 1 && selectedProductIndex < list.Count - 1)
                    selectedProductIndex++;
                break;

            case NavigationAction.Up:
                if (selectedProductIndex - cols >= 0)
                {
                    selectedProductIndex -= cols;
                }
                else
                {
                    
                    inProducts = false;
                    inOffers = true;

                    selectedOfferIndex = offers.Count - 3; 
                }

                break;

            case NavigationAction.Down:
                if (selectedProductIndex + cols < list.Count)
                    selectedProductIndex += cols;

                int visibleRows = (Console.WindowHeight - 20) / 4;
                int maxVisibleItems = visibleRows * cols;

                if (selectedProductIndex >= productScrollOffset + maxVisibleItems)
                    productScrollOffset += cols;

                break;

            case NavigationAction.Select:
                modalProduct = list[selectedProductIndex];
                modalVariants = (await _repo.GetAllAsync())
                    .Where(p => p.Name == modalProduct.Name)
                    .ToList();

                selectedSizeIndex = 0;
                selectedModalButton = 0;
                sizeErrorShown = false;
                inModal = true;
                break;
        }
    }
    
    
    private async Task HandleModalNavigation(NavigationAction action)
    {
        switch (action)
        {
            case NavigationAction.Select:

                if (selectedModalButton == 1)
                {
                    var selected = modalVariants[selectedSizeIndex];

                    if (selected.Inventory == 0)
                    {
                        TriggerSizeError();
                        return;
                    }

                    Console.Clear();
                    Console.WriteLine($"Added {selected.Name} ({selected.Size})");
                    _nav.GetAction();

                    ResetModal();
                }

                if (selectedModalButton == 2)
                    ResetModal();

                break;

            case NavigationAction.Back:
                ResetModal();
                break;
        }
    }

    private void ResetModal()
    {
        inModal = false;
        selectedSizeIndex = 0;
        selectedModalButton = 0;
        sizeErrorShown = false;
    }

    private void TriggerSizeError()
    {
        sizeErrorShown = true;
        lastErrorTime = DateTime.Now;
    }
    private async Task HandleSidebarNavigation(NavigationAction action)
    {
        
        if (categoriesOpen && selectedSidebarIndex == 0)
        {
            switch (action)
            {
                case NavigationAction.Up:
                    if (selectedCategoryIndex > 0)
                        selectedCategoryIndex--;
                    break;
                case NavigationAction.Right:
                    inSidebar = false;

                    inOffers = !categoryActive;
                    inProducts = categoryActive;
                    break;
                    
                case NavigationAction.Down:
                    if (selectedCategoryIndex < categories.Length - 1)
                        selectedCategoryIndex++;
                    break;

                case NavigationAction.Select:
                    selectedCategory = categories[selectedCategoryIndex];
                    categoryActive = true;

                    await LoadCategoryProducts(selectedCategory);

                    inSidebar = false;
                    inProducts = true;
                    inOffers = false; 
                    break;

                case NavigationAction.Back:
                    categoriesOpen = false; 
                    break;
            }

            return;
        }

        
        switch (action)
        {
            case NavigationAction.Up:
                if (selectedSidebarIndex > 0)
                    selectedSidebarIndex--;
                break;

            case NavigationAction.Down:
                if (selectedSidebarIndex < sidebarOptions.Length - 1)
                    selectedSidebarIndex++;
                break;

            case NavigationAction.Right:
                inSidebar = false;
                inOffers = !categoryActive;
                inProducts = categoryActive;
                break;

            case NavigationAction.Select:

                
                if (selectedSidebarIndex == 0)
                {
                    categoriesOpen = !categoriesOpen;
                    selectedCategoryIndex = 0;
                }

                
                if (selectedSidebarIndex == 1)
                {
                    categoryActive = false;
                    selectedCategory = null;

                    await LoadOffers();
                    await LoadProducts();

                    inSidebar = false;
                    inOffers = true;
                    inProducts = false;
                }

                break;
        }
    }
    private void HandleOffersNavigation(NavigationAction action)
    {
        int cols = 3;

        switch (action)
        {
            case NavigationAction.Left:
                if (selectedOfferIndex % cols > 0)
                    selectedOfferIndex--;
                else
                {
                    inOffers = false;
                    inSidebar = true;
                }
                break;

            case NavigationAction.Right:
                if (selectedOfferIndex % cols < cols - 1 && selectedOfferIndex < offers.Count - 1)
                    selectedOfferIndex++;
                break;

            case NavigationAction.Up:
                if (selectedOfferIndex - cols >= 0)
                    selectedOfferIndex -= cols;
                break;

            case NavigationAction.Down:
                if (selectedOfferIndex + cols < offers.Count)
                    selectedOfferIndex += cols;
                else
                {
                    inOffers = false;
                    inProducts = true;
                    selectedProductIndex = 0;
                }
                break;

            case NavigationAction.Select:
                Console.Clear();
                Console.WriteLine($"Added {offers[selectedOfferIndex].Name}");
                _nav.GetAction();
                break;
        }
    }


    public async Task Start()
    {
        Console.CursorVisible = false;

        await LoadOffers();
        await LoadProducts();

        while (true)
        {
            
            Console.Clear();

            DrawHeader();
            DrawSidebar();
            DrawProducts();

            if (!categoryActive)
            {
                DrawOffers();
            }

            if (inModal)
                DrawModal();

            var action = _nav.GetAction();

            if (inModal)
            {
                await HandleModalNavigation(action);
            }
            else if (inSidebar)
            {
                await HandleSidebarNavigation(action);
            }
            else if (inOffers)
            {
                HandleOffersNavigation(action);
            }
            else if (inProducts)
            {
                await HandleProductsNavigation(action);
            }
            
            if (action == NavigationAction.Back)
                return;
        }
    }
    
    private void DrawHeader()
    {
        string title = "Backends Clothing";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 1);
        Console.WriteLine(title);
    }
}