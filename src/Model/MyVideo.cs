namespace MScraper.Model;

//!! TODO:: REFACTOR THIS FILE AND RENAME !!//
public class MyVideo
{
    public string Name { get; set; }
    public string Link { get; set; }

    public MyVideo(string Name,string Link)
    {
        this.Name = Name;
        this.Link = Link;
    }
}