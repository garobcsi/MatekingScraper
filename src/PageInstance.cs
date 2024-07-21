using System.Text.RegularExpressions;
using MathScraper.Model;
using PuppeteerSharp;
using FFMpegCore;

namespace MathScraper;

public class PageInstance
{
    private PageInstance() {}
    
    private const int Width = 1920;
    private const int Height = 1080;
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

    private string StringFormat(string? str) => Regex.Replace(str??"", @"\t|\n|\r|JSHandle:", "").TrimEnd(' ').TrimStart(' ');
    private string CleanPath(string? str)
    {
        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + "?";
        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        return r.Replace(str ?? "", "");
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
                    
                    subjects.Add(new Subject() {Name = StringFormat(text.ToString()),Link = StringFormat(link.ToString())});
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
                    
                    subjects.Add(new Subject() {Name = StringFormat(text.ToString()),Link = StringFormat(link.ToString())});
                }

                return subjects;
                break;
            }
            case SubjectType.University :
            {
                List<Subject> subjects = new();
                { // First website
                    await Page.GoToAsync(Links.Base + Links.AllUniversitySubjects);
                    await Page.WaitForSelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > article > div > div > div > div > div > div > div");

                    var selections = await Page.QuerySelectorAllAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > article > div > div > div > div > div > div > div div.university");
                    foreach (var s in selections)
                    {
                        var hyperlinks = await s.QuerySelectorAllAsync("div > div.university-terms > div > a");
                        foreach (var h in hyperlinks)
                        {
                            var text = await h.GetPropertyAsync("textContent");
                            var link = await h.GetPropertyAsync("href");

                            string formatedLink = StringFormat(link.ToString());
                            if (formatedLink == Links.Base + Links.Thematics) continue;

                            subjects.Add(new Subject() { Name = StringFormat(text.ToString()), Link = formatedLink });
                        }
                    }
                }
                { // Second website
                    await Page.GoToAsync(Links.Base+Links.Thematics);
                    await Page.WaitForSelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > div > div.view-content");

                    var selections = await Page.QuerySelectorAllAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > div > div.view-content div");

                    foreach (var s in selections)
                    {
                        var hyperlinks = await s.QuerySelectorAllAsync("tbody > tr > td.views-field-field-related-categories > a");

                        foreach (var h in hyperlinks)
                        {
                            var text = await h.GetPropertyAsync("textContent");
                            var link = await h.GetPropertyAsync("href");
                            
                            subjects.Add(new Subject() { Name = StringFormat(text.ToString()), Link = StringFormat(link.ToString()) });

                        }
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
            
            subjects.Add(new Subject() {Name = StringFormat(text.ToString()),Link = StringFormat(link.ToString())});
        }
        
        return new Tuple<int, List<Subject>>(0,subjects); 
    }

    public async Task<List<SubSubject>> GetSubSubjects(Subject subject)
    {
        List<SubSubject> subSubjects = new();
        uint number=0;
        
        await Page.GoToAsync(subject.Link);
        await Page.WaitForSelectorAsync("#panel-page-mathsplain-video > div.panel-panel.panel-col-first > div > div.panel-pane.pane-block.pane-menu-block-1 > div > div > ul");
        var selections = await Page.QuerySelectorAllAsync(
            "#panel-page-mathsplain-video > div.panel-panel.panel-col-first > div > div.panel-pane.pane-block.pane-menu-block-1 > div > div > ul > li a");
        foreach (var s in selections)
        {
            var text = await s.GetPropertyAsync("textContent");
            var link = await s.GetPropertyAsync("href");
            
            subSubjects.Add(new SubSubject() {Name = StringFormat(text.ToString()),Link = StringFormat(link.ToString()),Number = ++number});
        }
        return subSubjects;
    }

    public async Task<List<Video>> GetVideos(SubSubject subSubject)
    {
        List<Video> videos = new();
        uint number = 0;
        
        await Page.GoToAsync(subSubject.Link);
        await Page.WaitForSelectorAsync("#isotope-container");
        var selections = await Page.QuerySelectorAllAsync("#isotope-container > div.isotope-element.video");
        foreach (var s in selections)
        {
            var text = await (await s.QuerySelectorAsync("a > div.video-box-content")).GetPropertyAsync("textContent");
            var link = await (await s.QuerySelectorAsync("a")).GetPropertyAsync("href");
            bool accessible = await s.QuerySelectorAsync("div.video-box-header-icons > div.video-free") != null;
            
            videos.Add(new Video() {Name = StringFormat(text.ToString()),Link = StringFormat(link.ToString()),Number = ++number,Accessible = accessible});
        }

        return videos;
    }

    public async Task<int> ScrapeVideo(Subject subject,SubSubject subSubject,Video video,CancellationToken cts)
    {
        if (!video.Accessible) return 1; // video is not scrapeable

        string path = Path.GetFullPath($"./data/{CleanPath(subject.Name)}/{(subSubject.Number)}-{CleanPath(subSubject.Name)}");
        string videoPath = Path.GetFullPath(path + $"/{video.Number}-{CleanPath(video.Name)}.metadata");

        DirectoryInfo folder = Directory.CreateDirectory(videoPath);

        await Page.GoToAsync(video.Link);
        await Page.WaitForSelectorAsync("body > div.panel-display.panel-1col.clearfix > div > div > div.panel-pane.pane-page-content.limited-wide > div > div > div > div > div > div.panel-pane.pane-entity-view.pane-node > div > div > div > div > div.field.field-name-field-swiffy-entity.field-type-entityreference.field-label-hidden");

        {//dowload audio
            string audioLink = await Page.EvaluateExpressionAsync<string>("hangforras");
            
            if (audioLink != "")
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(audioLink))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                               fileStream = new FileStream(Path.GetFullPath(videoPath+"/audio.mp3"), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            await contentStream.CopyToAsync(fileStream, cts);
                        }
                    }
                }
        }

        List<uint> chapterSlides = await Page.EvaluateExpressionAsync<List<uint>>("BogyoKulcsDiak");
        List<double> audioSlides = await Page.EvaluateExpressionAsync<List<double>>("HangKulcsIdok");

        uint slidesCount = chapterSlides.Last();
        
        {//download pngs
            string lastPng = await Page.EvaluateExpressionAsync<string>(@"
                (function() {
                    var canvas = document.querySelector('canvas');
                    return canvas.toDataURL('image/png');
                })();");
            
            await Page.EvaluateExpressionAsync("hangero = 0");
            
            while (true) // wait for first slide to load
            {
                await Page.EvaluateExpressionAsync($"Tekeres(1);");
                uint currentSlide = await Page.EvaluateExpressionAsync<uint>("AktualisDia");
                if (currentSlide == 1) break;
                await Task.Delay(500, cts);
            }
            
            for (int i = 1; i < slidesCount; i++)
            {
                string currentPng;
                await Page.EvaluateExpressionAsync($"Tekeres({i});");
                while (true)
                {
                    uint currentSlide = await Page.EvaluateExpressionAsync<uint>("AktualisDia");
                    currentPng = await Page.EvaluateExpressionAsync<string>(@"
                        (function() {
                            var canvas = document.querySelector('canvas');
                            return canvas.toDataURL('image/png');
                        })();");
                    if (currentSlide == i && lastPng != currentPng)
                    {
                        lastPng = currentPng;
                        break;
                    }
                }
                string base64 = currentPng.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64);
                await File.WriteAllBytesAsync(Path.GetFullPath(videoPath+$"/{i}.png"), imageBytes, cts);
            }
        }

        {//cut video
            string audioPath = Path.GetFullPath(videoPath + "/audio.mp3");
            bool audioExists = File.Exists(audioPath);
            
            uint muteAudioDuration = 5;
            
            using (StreamWriter writer = new StreamWriter(Path.GetFullPath(videoPath+"/imagelist.txt")))
            {
                if (audioExists)
                {
                    for (int i = 0; i < audioSlides.Count - 2; i++)
                    {
                        writer.WriteLine($"file './{i + 1}.png'");
                        string str = (audioSlides[i + 2] - audioSlides[i + 1]).ToString();
                        writer.WriteLine($"duration {str.Replace(',', '.')}");
                    }
                }
                else
                {
                    for (int i = 0; i < slidesCount-1; i++)
                    {
                        writer.WriteLine($"file './{i + 1}.png'");
                        writer.WriteLine($"duration {muteAudioDuration}");
                    }
                    writer.WriteLine($"file './{slidesCount-1}.png'");
                    writer.WriteLine($"duration {muteAudioDuration}");
                }
            }
            
            using (StreamWriter writer = new StreamWriter(Path.GetFullPath(videoPath+"/metadata.txt")))
            {
                writer.WriteLine(";FFMETADATA1");
                if (audioExists)
                {
                    for (int i = 0; i < audioSlides.Count - 1; i++)
                    {
                        writer.WriteLine($"[CHAPTER]");
                        writer.WriteLine($"TIMEBASE=1/1000");
                        writer.WriteLine($"START={(int)(audioSlides[i] * 1000)}"); 
                        writer.WriteLine($"END={(int)(audioSlides[i+1] * 1000)-1}"); 
                        writer.WriteLine($"title=Chapter {i + 1}");
                        writer.WriteLine();
                    }
                }
                else
                {
                    uint counter = 0;
                    for (int i = 0; i < slidesCount-1; i++)
                    {
                        writer.WriteLine($"[CHAPTER]");
                        writer.WriteLine($"TIMEBASE=1/1000");
                        writer.WriteLine($"START={(uint)(counter*1000)}");
                        writer.WriteLine($"END={(uint)(((counter += muteAudioDuration)*1000)-1)}"); 
                        writer.WriteLine($"title=Chapter {i + 1}");
                        writer.WriteLine();
                    }
                }
            }

            var ffmpegArgs = FFMpegArguments
                .FromFileInput(Path.GetFullPath(videoPath + "/imagelist.txt"), false, options => options
                    .WithCustomArgument("-f concat -safe 0 -r 1"));
            
            
            if (audioExists)
            {
                ffmpegArgs = ffmpegArgs.AddFileInput(audioPath);
            }
            
            var ffmpegProc = ffmpegArgs.AddFileInput(Path.GetFullPath(videoPath + "/metadata.txt"))
                .OutputToFile(Path.GetFullPath(path + $"/{video.Number}-{CleanPath(video.Name)}.mp4"), true, options =>
                {
                    options.WithVideoCodec("libx264")
                        .WithCustomArgument("-pix_fmt yuv420p")
                        .WithCustomArgument("-map 0:v:0");
            
                    options.WithCustomArgument("-c:v copy");
                    if (audioExists)
                    {
                        options.WithCustomArgument("-map 1:a:0")
                            .WithCustomArgument("-c:a aac");
                    }
            
                    options.WithCustomArgument("-map_metadata "+ (audioExists ? "2" : "1"));
                });
            await ffmpegProc.ProcessAsynchronously();
        }
        return 0; // scraped successfully
    }
}