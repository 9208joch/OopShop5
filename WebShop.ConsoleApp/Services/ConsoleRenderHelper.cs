using System;

public static class ConsoleRenderHelper
{
    public static void WriteCentered(string text, int y)
    {
        int x = (Console.WindowWidth - text.Length) / 2;
        Console.SetCursorPosition(x, y);
        Console.WriteLine(text);
    }
}
