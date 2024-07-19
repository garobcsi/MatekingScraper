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

List<Subject> boughtCourses = (await pai.GetMyCourses()).Item2; //load users bought courses

List<Subject> subjects;
Subject? selectedSubject = null;
{ // Select subject
    subjects = await pai.GetSubjects((SubjectType)selectedType);
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
}

{ //scrape video's data
    
}

{ //scrape videos
    
}

await pai.Page.CloseAsync();
await bwi.Browser.CloseAsync();