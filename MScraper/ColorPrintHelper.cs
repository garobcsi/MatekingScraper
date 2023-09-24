using System.Drawing;
using static MScraper.ColorHelper;

namespace MScraper;

public class ColorPrintHelper
{
    public static void WriteLine(string txt,ConsoleColor ForegroundColor,ConsoleColor BackgroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        Console.WriteLine(txt);
        Console.ResetColor();
    }
    public static void WriteLine(string txt,Color ForegroundColor,Color BackgroundColor)
    {
        Console.ForegroundColor = FindClosestConsoleColor(ForegroundColor);
        Console.BackgroundColor = FindClosestConsoleColor(BackgroundColor);
        Console.WriteLine(txt);
        Console.ResetColor();
    }
    public static void WriteLine(string txt,ConsoleColor ForegroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
        Console.WriteLine(txt);
        Console.ResetColor();
    }
    public static void WriteLine(string txt,Color ForegroundColor)
    {
        Console.ForegroundColor = FindClosestConsoleColor(ForegroundColor);
        Console.WriteLine(txt);
        Console.ResetColor();
    }

    public static void Write(string txt,ConsoleColor ForegroundColor,ConsoleColor BackgroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        Console.Write(txt);
        Console.ResetColor();
    }
    public static void Write(string txt,Color ForegroundColor,Color BackgroundColor)
    {
        Console.ForegroundColor = FindClosestConsoleColor(ForegroundColor);
        Console.BackgroundColor = FindClosestConsoleColor(BackgroundColor);
        Console.Write(txt);
        Console.ResetColor();
    }
    public static void Write(string txt,ConsoleColor ForegroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
        Console.Write(txt);
        Console.ResetColor();
    }
    public static void Write(string txt,Color ForegroundColor)
    {
        Console.ForegroundColor = FindClosestConsoleColor(ForegroundColor);
        Console.Write(txt);
        Console.ResetColor();
    }
}