namespace MScraper;

using dotenv.net;
using static PuppeteerInstance;
public class PuppeteerHelper
{
    public static async Task Login()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();
        
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
}