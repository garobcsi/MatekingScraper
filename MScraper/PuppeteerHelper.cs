using System.Drawing;
using MScraper.Model;
using PuppeteerSharp;

namespace MScraper;

using dotenv.net;
using static PuppeteerInstance;
public class PuppeteerHelper
{
    public static async Task Login()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();
        if (envVars["name"] == string.Empty || envVars["name"] == null ||
            envVars["password"] == string.Empty || envVars["password"] == null)
        {
            ColorPrintHelper.WriteLine("Name Or Password is empty.",ConsoleColor.Red);
            ExitHelper.Exit();
        }
        
        await page.WaitForSelectorAsync("#mathsplain-login-pane-opener");
        var user = await page.QuerySelectorAsync("#mathsplain-login-pane-opener");
        await user.ClickAsync();
        
        await page.TypeAsync("#edit-name",envVars["name"]);
        await page.TypeAsync("#edit-pass", envVars["password"]);
        var login = await page.QuerySelectorAsync("#edit-submit");
        await login.ClickAsync();
        await page.WaitForNavigationAsync();
        
        var error = await page.QuerySelectorAsync("#absolute-messages-messages>div.absolute-messages-message.absolute-messages-error>div.content");
        if (error == null )
        {
            ColorPrintHelper.WriteLine("Login Success !",ConsoleColor.Green);
        }
        else
        {
            ColorPrintHelper.WriteLine("Login Error !",ConsoleColor.Red);
            ExitHelper.Exit();
        }
    }

    public static async Task<List<MyCourse>> GetMyCourses()
    {
        await page.GoToAsync(Links.Base+Links.MyCourses);
        
        List<MyCourse> list = new List<MyCourse>();
        
        var Course = await page.QuerySelectorAsync("#mathsplain-mycourses-isotope-container>div.purchased");
        IElementHandle[] Courses = await Course.QuerySelectorAllAsync("div>div.isotope-link>a");
        IElementHandle[] Courses_Color = await Course.QuerySelectorAllAsync("div.isotope-element");

        if (Courses.Length == 0)
        {
            ColorPrintHelper.WriteLine("You did not buy any Courses !",ConsoleColor.Red);
            ExitHelper.Exit();
        }

        for (int i = 0; i < Courses.Length; i++)
        {
            var text = await Courses[i].GetPropertyAsync("textContent");
            var link = await Courses[i].GetPropertyAsync("href");
            var color = await Courses_Color[i].EvaluateFunctionAsync<string>(@"(element) => {
            const computedStyle = getComputedStyle(element);
            return computedStyle.backgroundColor;
            }");
            color = color.Remove(0, 4);
            color = color.Remove(color.Length-1,1);
            int[] colors = color.Split(",").Select(int.Parse).ToArray();
            list.Add(new MyCourse(await text.JsonValueAsync<string>(),await link.JsonValueAsync<string>(),Color.FromArgb(1,colors[0],colors[1],colors[2])));
        }
        
        return list;
    }
    
    public static async Task BrowserExit()
    {
        await browser.CloseAsync();
    }
    
}