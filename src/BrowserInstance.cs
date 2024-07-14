using System.Runtime.CompilerServices;
using dotenv.net;
using PuppeteerSharp;

namespace MathScraper;

public class BrowserInstance
{
    private BrowserInstance() {}
    private const int Width = 600;
    private const int Height = 600;
    
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
    
    public IBrowser? Browser { get; private set; } = null;
    public IPage? Page { get; private set; } = null; 
    
    public static async Task<BrowserInstance> Init()
    {
        BrowserInstance bwi = new BrowserInstance();
        bwi.Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = Headless });
        bwi.Page = await bwi.Browser.NewPageAsync();
        await bwi.Page.SetViewportAsync(new ViewPortOptions
        {
            Width = Width,
            Height = Height
        });
        await bwi.Page.GoToAsync(Links.Base);
        return bwi;
    }
    
    ~BrowserInstance()
    {
        Browser = null;
        Page = null;
    }
}