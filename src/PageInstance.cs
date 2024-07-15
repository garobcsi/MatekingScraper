using PuppeteerSharp;

namespace MathScraper;

public class PageInstance
{
    private PageInstance() {}
    
    private const int Width = 600;
    private const int Height = 600;
    public IPage Page { get; private set; }
    public static async Task<PageInstance> Init(BrowserInstance bwi)
    {
        PageInstance pai = new PageInstance();
        pai.Page = await bwi.Browser.NewPageAsync();
        await pai.Page.SetViewportAsync(new ViewPortOptions
        {
            Width = Width,
            Height = Height
        });
        return pai;
    }
}