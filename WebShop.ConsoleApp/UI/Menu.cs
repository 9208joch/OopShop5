using System;
using System.Linq;
using _1.WebShop.Core.Interfaces;
using _1.WebShop.Core.Entities;

namespace WebShop.ConsoleApp.UI;

public class Menu
{
    private readonly IProductRepository _repo;

    private int selectedIndex = 0;
    private int selectedOfferIndex = 0;
    private bool inOffers = true;

    private string[] options = new[]
    {
        "Home Page",
        "Browse Products",
        "Shopping Cart",
        "Admin",
        "Quit"
    };

    public Menu(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task Start()
    {
        Console.CursorVisible = false;

        var offers = await GetOffers();

        while (true)
        {
            Console.Clear();

            DrawHeader();
            DrawOffers(offers);
            DrawMenu();

            var key = Console.ReadKey(true).Key;

            if (inOffers)
            {
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        selectedOfferIndex = (selectedOfferIndex - 1 + offers.Count) % offers.Count;
                        break;

                    case ConsoleKey.RightArrow:
                        selectedOfferIndex = (selectedOfferIndex + 1) % offers.Count;
                        break;

                    case ConsoleKey.DownArrow:
                        inOffers = false;
                        selectedIndex = 0; // börja högst upp i menyn
                        break;

                    case ConsoleKey.Enter:
                        var product = offers[selectedOfferIndex];

                        Console.Clear();
                        Console.WriteLine($"Added {product.Name} to cart!");
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex == 0)
                        {
                            inOffers = true; // gå tillbaka till offers
                        }
                        else
                        {
                            selectedIndex--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        bool reload = await HandleSelection();

                        if (reload)
                        {
                            offers = await GetOffers();
                            inOffers = true;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        inOffers = true;
                        break;
                }
            }

            if (key == ConsoleKey.Backspace)
                return;
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
        int startY = 6;

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

            Console.ResetColor();

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
        int startY = 12;
        int startX = 5;

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
                Console.Clear();
                Console.WriteLine("Browse Products coming soon...");
                Console.ReadKey();
                break;

            case 2:
                Console.Clear();
                Console.WriteLine("Shopping Cart coming soon...");
                Console.ReadKey();
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