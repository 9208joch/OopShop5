using System;
using System.Linq;
using _1.WebShop.Core.Interfaces;
using _1.WebShop.Core.Entities;
using _1.WebShop.Application;           
using _1.WebShop.Application.Services;
using _2.WebShop.Application.Services;
using UI;

namespace WebShop.ConsoleApp.UI;

public class Menu
{
    private readonly ShopMenu _shop;
    private readonly IDistanceService _distanceService;


    private readonly ShoppingCartMenu _shoppingCartMenu;   //NK...

    public Menu(IProductRepository repo, ConsoleNavigationService nav, ShopMenu shop, ShoppingCartMenu shoppingCartMenu)   //NK...ShoppingCartMenu shoppingCartMenu
    {
        _repo = repo;
        _nav = nav;
        _shop = shop;
        _shoppingCartMenu = shoppingCartMenu;   //NK...
    }
    private readonly IProductRepository _repo;

    private int selectedIndex = 0;
    private int selectedOfferIndex = 0;
    private bool inOffers = true;

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

        // Adress
        Console.SetCursorPosition(center - address.Length / 2, 4);
        Console.WriteLine(address);

        // Avstånd (test-adress tills vidare)
        if (_distanceService != null)
        {
            try
            {
                string customerAddress = "Stockholm"; // placeholder<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                var distance = await _distanceService.GetDistanceToStoreAsync(customerAddress);

                string text = distance != null
                    ? $"Distance to store: {Math.Round(distance.Value, 1)} km"
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
    }


    public async Task Start()
    {
        Console.CursorVisible = false;

        var offers = await GetOffers();

        if (!offers.Any())
        {
            Console.WriteLine("No offers available.");
            _nav.GetAction();
            return;
        }
        
        while (true)
        {
            Console.Clear();

            DrawHeader();
            await DrawStoreInfo();
            DrawOffers(offers);
            DrawMenu();

            var action = _nav.GetAction();

            if (inOffers)
            {
                switch (action)
                {
                    case NavigationAction.Left:
                        selectedOfferIndex = (selectedOfferIndex - 1 + offers.Count) % offers.Count;
                        break;

                    case NavigationAction.Right:
                        selectedOfferIndex = (selectedOfferIndex + 1) % offers.Count;
                        break;

                    case NavigationAction.Down:
                        inOffers = false;
                        selectedIndex = 0;
                        break;

                    case NavigationAction.Select:
                        var product = offers[selectedOfferIndex];

                        Console.Clear();
                        Console.WriteLine($"Added {product.Name} to cart!");
                        Console.WriteLine("Press any key...");

                        _nav.GetAction(); 
                        break;

                    case NavigationAction.Back:
                        return;
                }
            }
            else
            {
                switch (action)
                {
                    case NavigationAction.Up:
                        if (selectedIndex == 0)
                            inOffers = true;
                        else
                            selectedIndex--;
                        break;

                    case NavigationAction.Down:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        break;

                    case NavigationAction.Select:
                        bool reload = await HandleSelection();

                        if (reload)
                        {
                            offers = await GetOffers();
                            inOffers = true;
                            selectedOfferIndex = 0;
                        }
                        break;

                    case NavigationAction.Left:
                        inOffers = true;
                        break;

                    case NavigationAction.Back:
                        return;
                }
            }
        }
    }

    private void DrawHeader()
    {
        string title = "Backends Clothing";
        string subtitle = "Over 1000 products!";

        int center = Console.WindowWidth / 2;

        Console.SetCursorPosition(center - title.Length / 2, 1);
        Console.WriteLine(title);

        Console.SetCursorPosition(center - subtitle.Length / 2, 2);
        Console.WriteLine(subtitle);
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

    private void DrawOffers(List<Product> offers)
    {
        int cardWidth = 30;
        int innerWidth = 28;

        int totalWidth = cardWidth * offers.Count;
        int startX = (Console.WindowWidth - totalWidth) / 2;
        int startY = 8;
        
        string title = "=== Great Offers ===";
        int center = Console.WindowWidth / 2;

        Console.SetCursorPosition(center - title.Length / 2, startY - 2);
        Console.WriteLine(title);

        for (int i = 0; i < offers.Count; i++)
        {
            var p = offers[i];
            int x = startX + (i * cardWidth);

            bool selected = i == selectedOfferIndex && inOffers;

            
            var lines = WrapText(p.Description, innerWidth).Take(2).ToList();

            int currentY = startY;

            
            if (selected)
                Console.BackgroundColor = ConsoleColor.DarkGray;

            // Name
            Console.SetCursorPosition(x, currentY++);
            Console.WriteLine(p.Name.PadRight(innerWidth));
            
            // Description (flera rader)
            foreach (var line in lines)
            {
                Console.SetCursorPosition(x, currentY++);
                Console.WriteLine(line.PadRight(innerWidth));
            }
            
            // Price
            Console.SetCursorPosition(x, currentY++);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Price: {p.Price} kr".PadRight(innerWidth));

            // Sale
            Console.SetCursorPosition(x, currentY++);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Sale: {p.SalePrice ?? p.Price} kr".PadRight(innerWidth));

            

            // Add to cart
            Console.SetCursorPosition(x, currentY);

            if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }

            Console.WriteLine("[Add to cart]".PadRight(innerWidth));

            Console.ResetColor();
        }
    }
    private List<string> WrapText(string text, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";
        
        foreach (var word in words)
        {
            if ((currentLine + word).Length > maxWidth)
            {
                lines.Add(currentLine.Trim());
                currentLine = "";
            }

            currentLine += word + " ";
        }

        if (!string.IsNullOrWhiteSpace(currentLine))
            lines.Add(currentLine.Trim());

        return lines;
    }
    private void DrawMenu()
    {
        int startY = 15;
        int startX = 15;

        for (int i = 0; i < options.Length; i++)
        {
            Console.SetCursorPosition(startX, startY + i);

            if (i == selectedIndex && !inOffers)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }

            Console.WriteLine(options[i]);
            Console.ResetColor();
        }
    }
    
    private async Task<bool> HandleSelection()
    {
        switch (selectedIndex)
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
                Console.WriteLine("Admin coming soon...");
                Console.ReadKey();
                break;

            case 4:
                Environment.Exit(0);
                break;
        }
        return false;
    }
}