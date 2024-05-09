namespace MScraper.Model;

public class MySubject
{
    public string Name { get; set; }
    public string Link { get; set; }

    public MySubject(string Name,string Link)
    {
        this.Name = Name;
        this.Link = Link;
    }
}