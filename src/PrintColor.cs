namespace MatekingScraper;

public class PrintColor
{
    public static void WriteLine(string str, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        if (foreground != null) Console.ForegroundColor = (ConsoleColor)foreground;
        if (background != null) Console.BackgroundColor = (ConsoleColor)background;
        Console.WriteLine(str);
        Console.ResetColor();
    }
    public static void Write(string str, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        if (foreground != null) Console.ForegroundColor = (ConsoleColor)foreground;
        if (background != null) Console.BackgroundColor = (ConsoleColor)background;
        Console.Write(str);
        Console.ResetColor();
    }
}