namespace MScraper;

public class ExitHelper
{
    public static void Exit()
    {
        ColorPrintHelper.WriteLine("\nAbort.",ConsoleColor.Red);
        Environment.Exit(0);
    }
}