namespace MScraper;

using dotenv.net;
public class PuppeteerHelper
{
    public static async Task Login()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();
        
        await PuppeteerInstance.page.WaitForSelectorAsync("#mathsplain-login-pane-opener");
        var user = await PuppeteerInstance.page.QuerySelectorAsync("#mathsplain-login-pane-opener");
        await user.ClickAsync();
        
        await PuppeteerInstance.page.TypeAsync("#edit-name",envVars["name"]);
        await PuppeteerInstance.page.TypeAsync("#edit-pass", envVars["password"]);
        var login = await PuppeteerInstance.page.QuerySelectorAsync("#edit-submit");
        await login.ClickAsync();
    }
}