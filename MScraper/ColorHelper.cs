using System.Drawing;

namespace MScraper;

public class ColorHelper
{
    public static ConsoleColor FindClosestConsoleColor(Color targetColor)
    {
        return Enum.GetValues(typeof(ConsoleColor))
            .Cast<ConsoleColor>()
            .OrderBy(color => CalculateColorDistance(targetColor, ConsoleColorToColor(color)))
            .First();
    }

    public static bool IsDarkColor(ConsoleColor consoleColor)
    {
        return consoleColor <= ConsoleColor.DarkYellow;
    }

    public static Color ConsoleColorToColor(ConsoleColor consoleColor)
    {
        switch (consoleColor)
        {
            case ConsoleColor.Black:
                return Color.Black;
            case ConsoleColor.DarkBlue:
                return Color.DarkBlue;
            case ConsoleColor.DarkGreen:
                return Color.DarkGreen;
            case ConsoleColor.DarkCyan:
                return Color.DarkCyan;
            case ConsoleColor.DarkRed:
                return Color.DarkRed;
            case ConsoleColor.DarkMagenta:
                return Color.DarkMagenta;
            case ConsoleColor.DarkYellow:
                return Color.DarkGoldenrod;
            case ConsoleColor.Gray:
                return Color.Gray;
            case ConsoleColor.DarkGray:
                return Color.DarkGray;
            case ConsoleColor.Blue:
                return Color.Blue;
            case ConsoleColor.Green:
                return Color.Green;
            case ConsoleColor.Cyan:
                return Color.Cyan;
            case ConsoleColor.Red:
                return Color.Red;
            case ConsoleColor.Magenta:
                return Color.Magenta;
            case ConsoleColor.Yellow:
                return Color.Yellow;
            case ConsoleColor.White:
                return Color.White;
            default:
                return Color.Black;
        }
    }

    public static double CalculateColorDistance(Color color1, Color color2)
    {
        int dr = color1.R - color2.R;
        int dg = color1.G - color2.G;
        int db = color1.B - color2.B;
        return Math.Sqrt(dr * dr + dg * dg + db * db);
    }
}