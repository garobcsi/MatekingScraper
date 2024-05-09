using MScraper;
using MScraper.Model;

if (!File.Exists(".env"))
{
    ColorPrintHelper.WriteLine("No .env file located.",ConsoleColor.Red);
    ExitHelper.Exit();
}
await new PuppeteerInstance().Init();
await PuppeteerHelper.Login();
List<MyCourse> MyCourses = await PuppeteerHelper.GetMyCourses();

await PuppeteerHelper.BrowserExit();
