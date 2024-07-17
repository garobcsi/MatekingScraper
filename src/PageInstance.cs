using MathScraper.Model;
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
    public async Task<int> Login(string username,string password)
    {
        await Page.GoToAsync(Links.Base);
        await Page.WaitForSelectorAsync("#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-last > div > div.panel-pane.pane-custom");
        if (await Page.QuerySelectorAsync("#mathsplain-user-menu-opener") != null) return 2; // User already logged in
        
        await Page.WaitForSelectorAsync("#mathsplain-login-pane-opener");
        var user = await Page.QuerySelectorAsync("#mathsplain-login-pane-opener");
        await user.ClickAsync();
        
        await Page.TypeAsync("#edit-name",username);
        await Page.TypeAsync("#edit-pass", password);
        var login = await Page.QuerySelectorAsync("#edit-submit");
        await login.ClickAsync();
        await Page.WaitForNavigationAsync();
        
        await Page.WaitForSelectorAsync("#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-last > div > div.panel-pane.pane-custom");
        if (await Page.QuerySelectorAsync("#mathsplain-user-menu-opener") == null) return 1; // Login Failed
        
        return 0; // Login successful
    }

    public async Task<List<Subject>> GetSubjects(SubjectType type)
    {
        await Page.GoToAsync(Links.Base);
        await Page.WaitForSelectorAsync("#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div");
        
        switch (type)
        {
            case SubjectType.Preschool :
            {
                List<Subject> subjects = new();
                var selections = await Page.QuerySelectorAllAsync(
                    "#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div > div.panel-pane.pane-views.pane-left-menu.elementary-menu > div > div > div > div > ul > li");
                foreach (var i in selections)
                {
                    var selection = await i.QuerySelectorAsync("li > div > a");
                    var link = await selection.GetPropertyAsync("href");
                    var text = await selection.GetPropertyAsync("textContent");
                    
                    subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
                }

                return subjects;
                break;
            }
            case SubjectType.HighSchool :
            {
                List<Subject> subjects = new();
                var selections = await Page.QuerySelectorAllAsync(
                    "#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div > div.panel-pane.pane-views.pane-left-menu.highschool-menu > div > div > div > div > ul > li");
                foreach (var i in selections)
                {
                    var selection = await i.QuerySelectorAsync("li > div > a");
                    var link = await selection.GetPropertyAsync("href");
                    var text = await selection.GetPropertyAsync("textContent");
                    
                    subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
                }

                return subjects;
                break;
            }
            case SubjectType.University :
            {
                
                break;
            }
        }
        
        return new List<Subject>();
    }
}