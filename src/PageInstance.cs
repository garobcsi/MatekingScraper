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
        if (username == String.Empty || password == String.Empty) return 1;
        
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
        switch (type)
        {
            case SubjectType.Preschool :
            {
                await Page.GoToAsync(Links.Base);
                await Page.WaitForSelectorAsync("#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div");
                
                List<Subject> subjects = new();
                var selections = await Page.QuerySelectorAllAsync(
                    "#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div > div.panel-pane.pane-views.pane-left-menu.elementary-menu > div > div > div > div > ul > li");
                foreach (var i in selections)
                {
                    var selection = await i.QuerySelectorAsync("li > div > a");
                    var text = await selection.GetPropertyAsync("textContent");
                    var link = await selection.GetPropertyAsync("href");
                    
                    subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
                }

                return subjects;
                break;
            }
            case SubjectType.HighSchool :
            {
                await Page.GoToAsync(Links.Base);
                await Page.WaitForSelectorAsync("#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div");
                
                List<Subject> subjects = new();
                var selections = await Page.QuerySelectorAllAsync(
                    "#mini-panel-mathsplain_header_1 > div.panel-panel.panel-col-first > div > div.panel-pane.pane-views.pane-left-menu.highschool-menu > div > div > div > div > ul > li");
                foreach (var i in selections)
                {
                    var selection = await i.QuerySelectorAsync("li > div > a");
                    var text = await selection.GetPropertyAsync("textContent");
                    var link = await selection.GetPropertyAsync("href");
                    
                    subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
                }

                return subjects;
                break;
            }
            case SubjectType.University :
            {
                await Page.GoToAsync(Links.Base+Links.AllUniversitySubjects);
                await Page.WaitForSelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > article > div > div > div > div > div > div > div");

                List<Subject> subjects = new();
                var selections = await Page.QuerySelectorAllAsync(
                    "body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > article > div > div > div > div > div > div > div div.university");
                foreach (var s in selections)
                {
                    var hyperlinks = await s.QuerySelectorAllAsync("div > div.university-terms > div > a");
                    foreach (var h in hyperlinks)
                    {
                        var text = await h.GetPropertyAsync("textContent");
                        var link = await h.GetPropertyAsync("href");
                        
                        subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
                    }
                }

                subjects = subjects.DistinctBy(subject => subject.Link).OrderBy(subject => subject.Name).ToList();

                return subjects;
                break;
            }
        }
        
        return new List<Subject>();
    }

    public async Task<Tuple<int,List<Subject>>> GetMyCourses()
    {
        await Page.GoToAsync(Links.Base + Links.MyCourses);
        await Page.WaitForSelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > div > div > div");
        if (await Page.QuerySelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > div > div > div > div.panel-pane.pane-custom.pane-1.message > div > div > p") != null) return new Tuple<int, List<Subject>>(1,new List<Subject>()); //you are not logged in
        
        List<Subject> subjects = new();
        var selections = await Page.QuerySelectorAllAsync("#mathsplain-mycourses-isotope-container>div.purchased > div > div.isotope-link > a");
        foreach (var s in selections)
        {
            var text = await s.GetPropertyAsync("textContent");
            var link = await s.GetPropertyAsync("href");
            
            subjects.Add(new Subject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
        }
        
        return new Tuple<int, List<Subject>>(0,subjects); 
    }

    public async Task<List<SubSubject>> GetSubSubjects(Subject subject)
    {
        List<SubSubject> subSubjects = new();
        await Page.GoToAsync(subject.Link);
        await Page.WaitForSelectorAsync("#panel-page-mathsplain-video > div.panel-panel.panel-col-first > div > div.panel-pane.pane-block.pane-menu-block-1 > div > div > ul");
        var selections = await Page.QuerySelectorAllAsync(
            "#panel-page-mathsplain-video > div.panel-panel.panel-col-first > div > div.panel-pane.pane-block.pane-menu-block-1 > div > div > ul > li a");
        foreach (var s in selections)
        {
            var text = await s.GetPropertyAsync("textContent");
            var link = await s.GetPropertyAsync("href");
            
            subSubjects.Add(new SubSubject() {Name = text.ToString().Remove(0,9),Link = link.ToString().Remove(0,9)});
        }
        return subSubjects;
    }
}