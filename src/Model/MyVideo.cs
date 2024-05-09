namespace MScraper.Model;

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