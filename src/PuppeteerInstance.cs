using dotenv.net;
using PuppeteerSharp;

namespace MathScraper;

public class PuppeteerInstance
{
    private readonly bool _headless = new Func<bool>(() =>
    {
        string headless = DotEnv.Read()["headless"];
        if (headless != String.Empty && bool.TryParse(headless, out bool result))
        {
            return result;
        }
        return true;
    })();

    private const int Width = 600;
    private const int Height = 600;

    public static IBrowser? Browser { get; private set; } = null;
    public static IPage? Page { get; private set; } = null;

    public async Task Init()
    {
        if (Browser != null && Page != null) return;
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        Browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = _headless });
        Page = await Browser.NewPageAsync();
        await Page.SetViewportAsync(new ViewPortOptions
        {
            Width = Width,
            Height = Height
        });
        await Page.GoToAsync(Links.Base);
    }
    ~PuppeteerInstance()
    {
        Browser = null;
        Page = null;
    }
}
