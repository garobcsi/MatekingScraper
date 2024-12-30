using dotenv.net;
using PuppeteerSharp;

namespace MatekingScraper;

public class BrowserInstance
{
    private BrowserInstance() {}
    
    private static readonly bool Headless = new Func<bool>(() =>
    {
        var env = DotEnv.Read();
        if (!env.ContainsKey("headless")) return true;
        if (bool.TryParse(env["headless"], out bool result))
        {
            return result;
        }
        return true;
    })();
    
    public IBrowser Browser { get; private set; }
    public static async Task<BrowserInstance> Init()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        BrowserInstance bwi = new BrowserInstance
        {
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = Headless })
        };
        return bwi;
    }
}