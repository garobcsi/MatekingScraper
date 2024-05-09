using PuppeteerSharp;

namespace MathScraper;

public class PuppeteerInstance
{
    private const bool Headless = false;

    private const int Width = 600;
    private const int Height = 600;

    public static IBrowser? Browser { get; private set; } = null;
    public static IPage? Page { get; private set; } = null;

    public async Task Init()
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        Browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = Headless });
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
