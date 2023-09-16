namespace MScraper;
using PuppeteerSharp;

public class PuppeteerInstance {

    public static BrowserFetcher browserFetcher;

    private int Width = 1920;
    private int Height = 1080;
    private bool Headless = false;
    
    public async void Init()
    {
        browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = false });
        await using var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = this.Width,
            Height = this.Height
        });
        await page.GoToAsync(Links.Base);
    }
}