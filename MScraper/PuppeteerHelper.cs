namespace MScraper;

using dotenv.net;
public class PuppeteerHelper
{
    public static async Task Login()
    {
        DotEnv.Load();
        var envVars = DotEnv.Read();

    }
}