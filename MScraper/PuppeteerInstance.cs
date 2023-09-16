namespace MScraper;
using PuppeteerSharp;

public class PuppeteerInstance {
    
    private bool Headless = false;
    private int Width = 500;
    private int Height = 500;

    public static IBrowser browser;
    public static IPage page;
    
    public async Task Init()
    {
        BrowserFetcher browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = false });
        page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = this.Width,
            Height = this.Height
        });
        await page.GoToAsync(Links.Base);
    }
}