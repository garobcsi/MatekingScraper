using MScraper;

await new PuppeteerInstance().Init();
await PuppeteerHelper.Login();

Console.ReadKey();