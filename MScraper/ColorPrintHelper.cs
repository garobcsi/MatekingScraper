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
    public static void WriteLine(string txt,ConsoleColor ForegroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
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
    public static void Write(string txt,ConsoleColor ForegroundColor)
    {
        Console.ForegroundColor = ForegroundColor;
        Console.Write(txt);
        Console.ResetColor();
    }
}