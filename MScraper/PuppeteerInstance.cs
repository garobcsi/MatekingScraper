namespace MScraper;
using PuppeteerSharp;

public class PuppeteerInstance {
    
    private bool Headless = false;
    // private int Width = 1920;
    // private int Height = 1080;
    private int Width = 600;
    private int Height = 600;
    
    public static IBrowser browser { get; private set; }
    public static IPage page{ get; private set; }
    
    public async Task Init()
    {
        BrowserFetcher browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = Headless });
        page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = this.Width,
            Height = this.Height
        });
        await page.GoToAsync(Links.Base);
    }
}