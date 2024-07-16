using MathScraper;
using PuppeteerSharp;

BrowserInstance bwi = await BrowserInstance.Init();

AsyncJobQueue jobQueue = new(true,1);
for (int i = 0; i < 1; i++)
{
    jobQueue.AddJob(async (id, token) =>
    {
        PageInstance pai = await PageInstance.Init(bwi);
        await pai.Page.CloseAsync();
    });
}

await jobQueue.WaitForAllJobsAsync();
await bwi.Browser.CloseAsync();