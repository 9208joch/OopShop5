using System;

public enum NavigationAction
{
    None,
    Up,
    Down,
    Left,
    Right,
    Select,
    Back
}
public class ConsoleNavigationService
{
    public NavigationAction GetAction()
    {
        var key = Console.ReadKey(true).Key;

        return key switch
        {
            ConsoleKey.UpArrow => NavigationAction.Up,
            ConsoleKey.DownArrow => NavigationAction.Down,
            ConsoleKey.LeftArrow => NavigationAction.Left,
            ConsoleKey.RightArrow => NavigationAction.Right,
            ConsoleKey.Enter => NavigationAction.Select,
            ConsoleKey.Backspace => NavigationAction.Back,
            _ => NavigationAction.None
        };
    }
}
