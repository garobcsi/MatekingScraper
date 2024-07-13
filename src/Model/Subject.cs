namespace MathScraper.Model;

public class Subject
{
    public string? Name { get; set; } = null;
    public string? Link { get; set; } = null;
    public List<SubSubject>? SubSubjects { get; set; } = null;
}