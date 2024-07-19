using dotenv.net;
using MathScraper;
using MathScraper.Model;

BrowserInstance bwi = await BrowserInstance.Init();
PageInstance pai = await PageInstance.Init(bwi);
var env = DotEnv.Read();

{ //Login the user
    if (env.ContainsKey("username") && env.ContainsKey("password"))
    {
        int err = await pai.Login(env["username"],env["password"]);
        switch (err)
        {
            case 0:
            {
                PrintColor.WriteLine("info: login success",ConsoleColor.Green);
                break;
            }
            case 1:
            {
                PrintColor.WriteLine("error: login failed",ConsoleColor.Red);
                PrintColor.WriteLine("warn: continuing without account (scraping functions may be limited)",ConsoleColor.Yellow);
                break;
            }
            case 2:
            {
                PrintColor.WriteLine("warn: user already logged in",ConsoleColor.Yellow);
                break;
            }
        }
    }
    else
    {
        PrintColor.WriteLine("warn: username or password not present .env",ConsoleColor.Yellow);
        PrintColor.WriteLine("warn: continuing without account (scraping functions may be limited)",ConsoleColor.Yellow);
    }
}
}

await pai.Page.CloseAsync();
await bwi.Browser.CloseAsync();