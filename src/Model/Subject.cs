namespace MathScraper.Model;

public class Subject
{
    public string Name { get; set; } = "";
    public string Link { get; set; } = "";
    public List<SubSubject>? SubSubjects { get; set; } = null;
}