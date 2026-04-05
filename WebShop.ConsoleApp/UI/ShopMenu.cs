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

    private bool saleActive = false;

    private string[] sidebarOptions = { "Sale", "Categories", "Home Page", "Shopping Cart" };
    private string[] categories = { "Sweater", "Shorts", "T-shirt", "Jeans", "Jacket" };

    
    private bool inModal = false;
    private Product? modalProduct = null;
    private List<Product> modalVariants = new();

    private int selectedSizeIndex = 0;
    private int selectedModalButton = 0;

    private bool sizeErrorShown = false;
    private DateTime lastErrorTime = DateTime.MinValue;

    private int offerScrollOffset = 0;




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
            //.Take(6) <-- test
            .ToList();

        selectedOfferIndex = 0;
    }
    private async Task LoadProducts()
    {
        var rnd = new Random();

        products = (await _repo.GetAllAsync())
            .Where(p => !p.IsOnSale)
            .GroupBy(p => p.Name)
            .Select(g => g.First())
            .OrderBy(x => rnd.Next())
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

        for (int i = 0; i < sidebarOptions.Length; i++)
        {
            Console.SetCursorPosition(x, y);

            bool selected = inSidebar && i == selectedSidebarIndex;

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
                string arrow = categoriesOpen ? "▲" : "▼";
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
           
            if (i == 1 && categoriesOpen)
            {
                for (int j = 0; j < categories.Length; j++)
                {
                    Console.SetCursorPosition(x + 2, y);

                    bool catSelected =
                        inSidebar &&
                        selectedSidebarIndex == 1 &&
                        selectedCategoryIndex == j;

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

        string title = saleActive ? "=== Sale ===" : "=== Great Offers ===";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, startY - 2);
        Console.WriteLine(title);

        var list = saleActive ? offers : offers.Take(6).ToList();

        int visibleRows = (Console.WindowHeight - startY) / 7;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + offerScrollOffset;
            if (index >= list.Count) break;

            var p = list[index];

            int col = i % 3;
            int row = i / 3;

            int x = startX + col * cardWidth;
            int y = startY + row * 7;

            bool isSelected = index == selectedOfferIndex && inOffers;

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
        bool canScrollDown = offerScrollOffset + itemsPerPage < list.Count;
        bool canScrollUp = offerScrollOffset > 0;

        if (saleActive && canScrollDown)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, Console.WindowHeight - 1);
            Console.Write("[More ↓]");
        }

        if (saleActive && canScrollUp)
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

        int startY = categoryActive ? 6 : 20;
        int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;
        int maxHeight = Console.WindowHeight - 2;

        var list = categoryActive ? filteredProducts : products;

        int visibleRows = (Console.WindowHeight - startY) / 4;
        int itemsPerPage = visibleRows * 3;

        for (int i = 0; i < itemsPerPage; i++)
        {
            int index = i + productScrollOffset;
            if (index >= list.Count) break;

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
            Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));

            Console.SetCursorPosition(x, y + 2);

            if (isSelected)
                Highlight();

            Console.WriteLine("[More info]".PadRight(innerWidth));
            Console.ResetColor();
        }

        bool canScrollDown = productScrollOffset + itemsPerPage < list.Count;
        bool canScrollUp = productScrollOffset > 0;

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


    private void DrawModal()
    {
        int width = 70;
        int height = Math.Min(25, 12 + modalVariants.Count); //  dynamisk höjd

        int x = (Console.WindowWidth - width) / 2;
        int y = 3;

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

        //  Sizes vänster
        for (int i = 0; i < modalVariants.Count; i++)
        {
            int posY = y + 8 + i;
            if (posY >= y + height - 2) break;

            var v = modalVariants[i];

            Console.SetCursorPosition(x + 4, posY);

            string text = $"{v.Size}".PadRight(10) + $"({v.Inventory})";

            if (v.Inventory == 0)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            if (selectedModalButton == 0 && i == selectedSizeIndex)
                Highlight();

            Console.WriteLine(text);
            Console.ResetColor();
        }

        //  Knappar till HÖGER
        int buttonX = x + width - 20;

        Console.SetCursorPosition(buttonX, y + 10);
        if (selectedModalButton == 1)
            Highlight();
        Console.WriteLine("[Add to cart]");
        Console.ResetColor();

        Console.SetCursorPosition(buttonX, y + 12);
        if (selectedModalButton == 2)
            Highlight();
        Console.WriteLine("[Back]");
        Console.ResetColor();

        // Error
        if (sizeErrorShown && (DateTime.Now - lastErrorTime).TotalSeconds < 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x + 2, y + height - 2);
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

        if (list.Count == 0)
            return;

        int cols = 3;

        switch (action)
        {
            case NavigationAction.Left:
                inSidebar = true;
                inProducts = false;
                break;

            case NavigationAction.Right:
                if (selectedProductIndex % cols < cols - 1 &&
                    selectedProductIndex < list.Count - 1)
                {
                    selectedProductIndex++;
                }
                break;

            case NavigationAction.Up:

                // Gå till Offers om högst upp
                if (!categoryActive && !saleActive &&
                    selectedProductIndex < cols &&
                    productScrollOffset == 0)
                {
                    inProducts = false;
                    inOffers = true;
                    selectedOfferIndex = 0;
                    return;
                }

                if (selectedProductIndex - cols >= 0)
                    selectedProductIndex -= cols;

                // Scroll upp
                if (selectedProductIndex < productScrollOffset)
                    productScrollOffset -= cols;

                if (productScrollOffset < 0)
                    productScrollOffset = 0;

                break;

            case NavigationAction.Down:

                if (selectedProductIndex + cols < list.Count)
                    selectedProductIndex += cols;

                // Samma logik som DrawProducts
                int startY = categoryActive ? 6 : 20;
                int visibleRows = (Console.WindowHeight - startY) / 4;
                int maxVisibleItems = visibleRows * cols;

                if (selectedProductIndex >= productScrollOffset + maxVisibleItems)
                    productScrollOffset += cols;

                break;

            case NavigationAction.Select:

                //  Safety check
                if (selectedProductIndex < 0 || selectedProductIndex >= list.Count)
                    return;

                modalProduct = list[selectedProductIndex];

                modalVariants = (await _repo.GetAllAsync())
                    .Where(p => p.Name == modalProduct.Name)
                    .OrderBy(p => p.Size) 
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
            case NavigationAction.Up:
                if (selectedModalButton == 0 && selectedSizeIndex > 0)
                    selectedSizeIndex--;
                else if (selectedModalButton > 0)
                    selectedModalButton--;
                break;
            case NavigationAction.Down:
                if (selectedModalButton == 0 && selectedSizeIndex < modalVariants.Count - 1)
                    selectedSizeIndex++;
                else if (selectedModalButton < 2)
                    selectedModalButton++;
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
        
  
        // DROPDOWN AKTIV
        
        if (categoriesOpen && selectedSidebarIndex == 1)
        {
            switch (action)
            {
                case NavigationAction.Up:
                    if (selectedCategoryIndex > 0)
                        selectedCategoryIndex--;
                    break;

                case NavigationAction.Down:
                    if (selectedCategoryIndex < categories.Length - 1)
                        selectedCategoryIndex++;
                    break;

                case NavigationAction.Select:
                    selectedCategory = categories[selectedCategoryIndex];
                    categoryActive = true;

                    await LoadCategoryProducts(selectedCategory);

                    categoriesOpen = false;     // stäng dropdown
                    inSidebar = false;
                    inProducts = true;
                    inOffers = false;

                    productScrollOffset = 0;
                    selectedProductIndex = 0;
                    break;

                case NavigationAction.Back:
                case NavigationAction.Left:
                    categoriesOpen = false;     // stäng dropdown
                    break;
            }

            return;
        }

        // VANLIG SIDEBAR
        
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
                
                
                //  SALE
                
                if (selectedSidebarIndex == 0)
                {
                    saleActive = true;
                    categoryActive = false;

                    offerScrollOffset = 0; 
                    selectedOfferIndex = 0;

                    inSidebar = false;
                    inOffers = true;
                    inProducts = false;

                    await LoadOffers();
                    return;
                }

                
                //  CATEGORIES
                
                if (selectedSidebarIndex == 1)
                {
                    categoriesOpen = !categoriesOpen;
                    selectedCategoryIndex = 0;
                    return;
                }

                
                // HOME PAGE
                
                if (selectedSidebarIndex == 2)
                {
                    // 🔥 reset ALL state
                    saleActive = false;
                    categoryActive = false;
                    categoriesOpen = false;

                    inOffers = false;
                    inProducts = false;
                    inSidebar = false;
                    inModal = false;

                    return; // lämnar Start()
                }

               
                // SHOPPING CART
              
                if (selectedSidebarIndex == 3)
                {
                    Console.Clear();
                    Console.WriteLine("Shopping cart coming soon...");
                    _nav.GetAction();
                    return;
                }

                break;
        }
    }
    private void HandleOffersNavigation(NavigationAction action)
    {
        var list = saleActive ? offers : offers.Take(6).ToList();

        int cols = 3;

        int startY = 6;
        int visibleRows = (Console.WindowHeight - startY) / 7;
        int itemsPerPage = visibleRows * cols;

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
                if (selectedOfferIndex % cols < cols - 1 &&
                    selectedOfferIndex < list.Count - 1)
                    selectedOfferIndex++;
                break;

            case NavigationAction.Up:

                if (selectedOfferIndex - cols >= 0)
                    selectedOfferIndex -= cols;

                if (selectedOfferIndex < offerScrollOffset)
                    offerScrollOffset -= cols;

                if (offerScrollOffset < 0)
                    offerScrollOffset = 0;

                break;

            case NavigationAction.Down:

                if (selectedOfferIndex + cols < list.Count)
                {
                    selectedOfferIndex += cols;
                }
                else
                {
                    //  Endast i HOME (inte i sale)
                    if (!saleActive)
                    {
                        inOffers = false;
                        inProducts = true;
                        selectedProductIndex = 0;
                    }
                }

                //  Scroll (endast i sale)
                if (saleActive)
                {
                    if (selectedOfferIndex >= offerScrollOffset + itemsPerPage)
                        offerScrollOffset += cols;
                }

                break;

            case NavigationAction.Select:
                Console.Clear();
                Console.WriteLine($"Added {list[selectedOfferIndex].Name}");
                _nav.GetAction();
                break;
        }
    }


    public async Task Start()
    {
        Console.CursorVisible = false;

        await LoadOffers();
        await LoadProducts();

        // säker start
        inOffers = true;
        inProducts = false;
        inSidebar = false;
        inModal = false;
        categoriesOpen = false;
        categoryActive = false;
        saleActive = false;

        bool running = true;

        while (running)
        {
            Console.Clear();

            DrawHeader();
            DrawSidebar();

            //  RENDER CONTENT
            
            if (saleActive)
            {
                DrawOffers(); // endast reavaror
            }
            else if (!categoryActive)
            {
                DrawOffers();
                DrawProducts();
            }
            else
            {
                DrawProducts();
            }

            // Modal overlay (över allt)
            if (inModal)
                DrawModal();

            var action = _nav.GetAction();

            
            //  GLOBAL BACK
           
            if (action == NavigationAction.Back)
            {
                categoriesOpen = false;

                if (inModal)
                {
                    inModal = false;
                    continue;
                }

                if (inProducts || inOffers)
                {
                    inSidebar = true;
                    inProducts = false;
                    inOffers = false;
                    saleActive = false;
                    continue;
                }

                //lämna ShopMenu
                running = false;
                break;
            }

         
            if (inModal)
            {
                await HandleModalNavigation(action);
            }
            else if (inSidebar)
            {
                await HandleSidebarNavigation(action);

                //  Om sidebar valde Home → lämna direkt
                if (!inSidebar && !inOffers && !inProducts && !inModal)
                {
                    running = false;
                    break;
                }
            }
            else if (inOffers)
            {
                HandleOffersNavigation(action);
            }
            else if (inProducts)
            {
                await HandleProductsNavigation(action);
            }
        }

        //rensa UI när lämnar
        Console.Clear();
    }

    private void DrawHeader()
    {
        string title = "Backends Clothing";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 1);
        Console.WriteLine(title);
    }
}