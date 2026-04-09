using System;
using System.Collections.Generic;
using System.Text;

namespace _4.WebShop.ConsoleApp.UI
{
    public class ListSelector<T>    // Klass för att skapa generisk meny där man kan bläddra upp/ner i en lista i konsolen. Välja med Enter eller avbryta med Backspace
    {
        private readonly ConsoleNavigationService _navigation;

        public ListSelector(ConsoleNavigationService navigation)
        {
            _navigation = navigation;
        }

        public T Select(
            List<T> items,
            Func<T, string> display,
            string title = "Select an option")
        {
            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== {title.ToUpper()} ===\n");

                for (int i = 0; i < items.Count; i++)
                {
                    bool selected = i == selectedIndex;

                    if (selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(display(items[i]).PadRight(40));

                    Console.ResetColor();
                }

                Console.WriteLine("\nENTER = select | BACKSPACE = cancel");

                var action = _navigation.GetAction();

                switch (action)
                {
                    case NavigationAction.Up:
                        selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : items.Count - 1;
                        break;

                    case NavigationAction.Down:
                        selectedIndex = selectedIndex < items.Count - 1 ? selectedIndex + 1 : 0;
                        break;

                    case NavigationAction.Select:
                        return items[selectedIndex];

                    case NavigationAction.Back:
                        return default;
                }
            }
        }
    }
}