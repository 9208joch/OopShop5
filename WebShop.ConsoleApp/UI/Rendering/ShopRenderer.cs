using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using _1.WebShop.Core.Entities;
using WebShop.ConsoleApp.UI.State;

namespace WebShop.ConsoleApp.UI.Rendering
{
    public class ShopRenderer
    {
        public void DrawHeader()
        {
            string title = "Backends Clothing";
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 1);
            Console.WriteLine(title);
        }
        public void DrawProducts(
            ShopState state,
            List<Product> searchResults)
        {
            int cardWidth = 30;
            int innerWidth = 28;

            int startY = state.CategoryActive ? 6 : 20;
            int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;
            int maxHeight = Console.WindowHeight - 2;

            var list = state.SearchActive
                ? searchResults
                : (state.CategoryActive ? state.FilteredProducts : state.Products);

            if (list.Count == 0)
            {
                Console.SetCursorPosition(startX, startY);
                Console.WriteLine("No products found...");
                return;
            }

            int visibleRows = (Console.WindowHeight - startY) / 4;
            int itemsPerPage = visibleRows * 3;

            for (int i = 0; i < itemsPerPage; i++)
            {
                int index = i + state.ProductScrollOffset;
                if (index >= list.Count) break;

                var p = list[index];

                int col = i % 3;
                int row = i / 3;

                int x = startX + col * cardWidth;
                int y = startY + row * 4;

                bool isSelected = index == state.SelectedProductIndex && state.InProducts;

                if (isSelected)
                    Console.BackgroundColor = ConsoleColor.DarkGray;

                Console.SetCursorPosition(x, y);
                Console.WriteLine(p.Name.PadRight(innerWidth));

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

                Console.SetCursorPosition(x, y + 2);

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine("[More info]".PadRight(innerWidth));
                Console.ResetColor();
            }

            bool canScrollDown = state.ProductScrollOffset + itemsPerPage < list.Count;
            bool canScrollUp = state.ProductScrollOffset > 0;

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
        public void DrawOffers(ShopState state)
        {
            int cardWidth = 30;
            int innerWidth = 28;

            int startX = (Console.WindowWidth - (cardWidth * 3)) / 2;
            int startY = 6;

            string title = state.SaleActive ? "=== Sale ===" : "=== Great Offers ===";
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, startY - 2);
            Console.WriteLine(title);

            var list = state.SaleActive
                ? state.Offers
                : state.Offers.Take(6).ToList();

            int visibleRows = (Console.WindowHeight - startY) / 7;
            int itemsPerPage = visibleRows * 3;

            for (int i = 0; i < itemsPerPage; i++)
            {
                int index = i + state.OfferScrollOffset;
                if (index >= list.Count) break;

                var p = list[index];

                int col = i % 3;
                int row = i / 3;

                int x = startX + col * cardWidth;
                int y = startY + row * 7;

                bool isSelected = index == state.SelectedOfferIndex && state.InOffers;

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
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine("[Add to cart]".PadRight(innerWidth));
                Console.ResetColor();
            }

            bool canScrollDown = state.OfferScrollOffset + itemsPerPage < list.Count;
            bool canScrollUp = state.OfferScrollOffset > 0;

            if (state.SaleActive && canScrollDown)
            {
                Console.SetCursorPosition(Console.WindowWidth - 10, Console.WindowHeight - 1);
                Console.Write("[More ↓]");
            }

            if (state.SaleActive && canScrollUp)
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

        public void DrawSidebar(
    ShopState state,
    string[] sidebarOptions,
    string[] categories)
        {
            int x = 2;
            int y = 5;

            for (int i = 0; i < sidebarOptions.Length; i++)
            {
                Console.SetCursorPosition(x, y);

                bool selected = state.InSidebar && i == state.SelectedSidebarIndex;

                if (selected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ResetColor();
                }

                if (i == 0)
                {
                    if (!selected)
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("Sale");
                }
                else if (i == 1)
                {
                    string arrow = state.CategoriesOpen ? "▲" : "▼";
                    Console.WriteLine($"Categories {arrow}");
                }
                else
                {
                    Console.WriteLine(sidebarOptions[i]);
                }

                Console.ResetColor();
                y++;

                // CATEGORY DROPDOWN
                if (i == 1 && state.CategoriesOpen)
                {
                    for (int j = 0; j < categories.Length; j++)
                    {
                        Console.SetCursorPosition(x + 2, y);

                        bool catSelected =
                            state.InSidebar &&
                            state.SelectedSidebarIndex == 1 &&
                            state.SelectedCategoryIndex == j;

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
        public void DrawModal(
    ShopState state,
    List<Product> modalVariants,
    bool sizeErrorShown,
    DateTime lastErrorTime)
        {
            int width = 70;
            int height = Math.Min(25, 12 + modalVariants.Count);

            int x = (Console.WindowWidth - width) / 2;
            int y = 3;

            // Bakgrund
            Console.BackgroundColor = ConsoleColor.DarkGray;

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(new string(' ', width));
            }

            Console.ResetColor();

            void WriteLineInModal(int posX, int posY, string text)
            {
                Console.SetCursorPosition(posX, posY);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(text.PadRight(width - 4));
                Console.ResetColor();
            }

            // Titel
            WriteLineInModal(x + 2, y + 1, state.ModalProduct!.Name);

            // Beskrivning
            WriteLineInModal(x + 2, y + 3, state.ModalProduct.Description);

            // Pris
            WriteLineInModal(x + 2, y + 5, $"Price: {state.ModalProduct.Price} kr");

            // Label
            WriteLineInModal(x + 2, y + 7, "Sizes:");

            // SIZES
            for (int i = 0; i < modalVariants.Count; i++)
            {
                int posY = y + 8 + i;
                if (posY >= y + height - 2) break;

                var v = modalVariants[i];

                Console.SetCursorPosition(x + 4, posY);

                string text = $"{v.Size,-10} ({v.Inventory,2})";

                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;

                if (v.Inventory == 0)
                    Console.ForegroundColor = ConsoleColor.Red;

                if (state.SelectedSize == i)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                if (state.SelectedModalButton == 0 && i == state.SelectedSizeIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(text.PadRight(20));
                Console.ResetColor();
            }

            int buttonX = x + width - 20;

            // Add to cart
            Console.SetCursorPosition(buttonX, y + 10);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            if (state.SelectedModalButton == 1)
                Console.BackgroundColor = ConsoleColor.Gray;

            Console.WriteLine("[Add to cart]");
            Console.ResetColor();

            // Back
            Console.SetCursorPosition(buttonX, y + 12);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;

            if (state.SelectedModalButton == 2)
                Console.BackgroundColor = ConsoleColor.Gray;

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
        public void DrawSearchBox(ShopState state)
        {
            int width = 50;
            int x = (Console.WindowWidth - width) / 2;
            int y = 5;

            // Titel
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Search products:");

            // Input box
            Console.SetCursorPosition(x, y + 1);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;

            string displayText = state.SearchQuery;

            if (displayText.Length > width)
                displayText = displayText[^width..];

            Console.Write(displayText.PadRight(width));

            Console.ResetColor();

            int cursorPos = Math.Min(displayText.Length, width - 1);
            Console.SetCursorPosition(x + cursorPos, y + 1);

            Console.CursorVisible = state.SearchTyping;
        }












    }
}