namespace MathScraper.Model;

public class SubSubject
{
    public string Name { get; set; } = "";
    public string Link { get; set; } = "";
    public List<string>? SubPages { get; set; } = null;
    public List<Video>? Videos { get; set; } = null;
}