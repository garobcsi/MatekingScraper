using MScraper;

if (!File.Exists(".env"))
{
    ColorPrintHelper.WriteLine("No .env file located.",ConsoleColor.Red);
    ExitHelper.Exit();
}
await new PuppeteerInstance().Init();
await PuppeteerHelper.Login();

Console.ReadKey();