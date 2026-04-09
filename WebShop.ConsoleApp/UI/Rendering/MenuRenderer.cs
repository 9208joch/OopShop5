using System;
using System.Collections.Generic;
using System.Text;
using WebShop.ConsoleApp.UI.State;

namespace WebShop.ConsoleApp.UI.Rendering
{
    public class MenuRenderer
    {
        
        public void DrawOffers(MenuState state)
        {
            var offers = state.Offers;

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
                
                bool selected = i == state.SelectedOfferIndex && state.InOffers;

                var lines = WrapText(p.Description, innerWidth).Take(2).ToList();

                int currentY = startY;

                if (selected)
                    Console.BackgroundColor = ConsoleColor.DarkGray;

                // Name
                Console.SetCursorPosition(x, currentY++);
                Console.WriteLine($"{p.Name} ({p.Size})".PadRight(innerWidth));

                // Description
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

                // Button
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
        public void DrawMenu(MenuState state, string[] options)
        {
            int startY = 15;
            int startX = 15;

            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);

                if (i == state.SelectedIndex && !state.InOffers)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);
                Console.ResetColor();
            }
        }
    }
}
