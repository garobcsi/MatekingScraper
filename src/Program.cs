using dotenv.net;
using MathScraper;
using MathScraper.Model;

BrowserInstance bwi = await BrowserInstance.Init();
if (bwi.Browser.IsConnected) PrintColor.WriteLine("info: browser instance started successfully",ConsoleColor.Green);
else PrintColor.WriteLine("error: browser instance failed to start",ConsoleColor.Red);

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

SubjectType? selectedType = null;
{ // User selects subject type
    Console.WriteLine("\nSelect Subject Type\n");
    Console.WriteLine("1. Preschool");
    Console.WriteLine("2. HighSchool");
    Console.WriteLine("3. University");
    
    while (true)
    {
        Console.ResetColor();
        Console.Write("Select: ");
        var select = Console.ReadLine();
        if (int.TryParse(select, out int s))
        {
            switch (s)
            {
                case 1:
                {
                    selectedType = SubjectType.Preschool;
                    break;
                }
                case 2:
                {
                    selectedType = SubjectType.HighSchool;
                    break;
                }
                case 3:
                {
                    selectedType = SubjectType.University;
                    break; 
                }
            }
        } 
        if (selectedType != null) break;
        PrintColor.WriteLine("Invalid Input !",ConsoleColor.Red);
    }
}

List<Subject> boughtCourses;
{
    var tmp = await pai.GetMyCourses(); //load users bought courses
    boughtCourses = tmp.Item2;
    if (tmp.Item1 == 0) PrintColor.WriteLine("info: Bought Subjects loaded successfully",ConsoleColor.Green);
    if (tmp.Item1 == 1) PrintColor.WriteLine("warn: Bought Subjects could not be loaded (user is not logged in)",ConsoleColor.Yellow);
}

List<Subject> subjects;
Subject? selectedSubject = null;
{ // Select subject
    subjects = await pai.GetSubjects((SubjectType)selectedType);
    PrintColor.WriteLine("info: Subjects loaded successfully",ConsoleColor.Green);
    
    Console.WriteLine("\nSelect Subject\n");
    
    var bought = boughtCourses.Select(subject => subject.Link).ToHashSet();
    for (int i = 0; i < subjects.Count; i++)
    {
        PrintColor.WriteLine($"{i+1}: {subjects[i].Name}",bought.Contains(subjects[i].Link) ? ConsoleColor.DarkCyan: null);
    }

    while (true)
    {
        Console.ResetColor();
        Console.Write("Select: ");
        var select = Console.ReadLine();
        if (int.TryParse(select, out int s) && 1 <= s && subjects.Count >= s)
        {
            selectedSubject = subjects[s - 1];
        }
        if (selectedSubject != null) break;
        PrintColor.WriteLine("Invalid Input !",ConsoleColor.Red);
    }
}

{ //scrape subject's data
    selectedSubject.SubSubjects = await pai.GetSubSubjects(selectedSubject);
    PrintColor.WriteLine("info: Sub Subjects loaded successfully",ConsoleColor.Green);
}

{ //scrape video's data
    AsyncJobQueue jobQueue = new AsyncJobQueue(true, 5);
    foreach (var s in selectedSubject.SubSubjects)
    {
        jobQueue.AddJob(async (id,token) =>
        {
            PageInstance p = await PageInstance.Init(bwi);
            s.Videos = await p.GetVideos(s);
            await p.Page.CloseAsync();
        });
    }
    await jobQueue.WaitForAllJobsAsync();
    PrintColor.WriteLine("info: Videos Data loaded successfully",ConsoleColor.Green);
}

{ //scrape videos
    PrintColor.WriteLine("info: Video scraping started",ConsoleColor.Green);
    
    AsyncJobQueue jobQueue = new AsyncJobQueue(new Func<bool>(() =>
    {
        var env = DotEnv.Read();
        if (!env.ContainsKey("enableJobQueue")) return true;
        if (bool.TryParse(env["enableJobQueue"], out bool result))
        {
            return result;
        }
        return true;
    })(), new Func<int>(() =>
    {
        var env = DotEnv.Read();
        if (!env.ContainsKey("maxJobQueueCount")) return 2;
        if (int.TryParse(env["maxJobQueueCount"], out int result))
        {
            return result;
        }
        return 2;
    })());
    
    foreach (var s in selectedSubject.SubSubjects)
    {
        if (s.Videos != null)
            foreach (var v in s.Videos)
            {
                if (!v.Accessible) continue;
                jobQueue.AddJob(async (id, token) =>
                {
                    Console.WriteLine($"info: Job {id} started");
                    PageInstance p = await PageInstance.Init(bwi);
                    
                    await p.ScrapeVideo(id,selectedSubject,s,v,token);
                    
                    await p.Page.CloseAsync();
                    Console.WriteLine($"info: Job {id} finished");
                });
            }
    }

    PrintColor.WriteLine($"info: {jobQueue.RunningJobsCount} jobs started",ConsoleColor.Green);
    
    await jobQueue.WaitForAllJobsAsync();
    if (jobQueue.FailedJobsCount == 0) PrintColor.WriteLine("info: Videos scraped successfully",ConsoleColor.Green);
    else PrintColor.WriteLine("error: Videos scraped unsuccessfully", ConsoleColor.Red);
}

await pai.Page.CloseAsync();
await bwi.Browser.CloseAsync();