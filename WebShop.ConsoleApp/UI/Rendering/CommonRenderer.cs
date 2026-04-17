using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.ConsoleApp.UI.Rendering
{
    public class CommonRenderer
    {
        public void DrawHeader()
        {
            string title = "Backends Clothing";
            string subtitle = "Over 1000 products!";

            int center = Console.WindowWidth / 2;

            Console.SetCursorPosition(center - title.Length / 2, 1);
            Console.WriteLine(title);

            if (!string.IsNullOrEmpty(subtitle))
            {
                Console.SetCursorPosition(center - subtitle.Length / 2, 2);
                Console.WriteLine(subtitle);
            }
        }
    }
}
