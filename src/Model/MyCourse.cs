using System.Drawing;

namespace MScraper.Model;

public class MyCourse
{
    public string Name { get; set; }
    public string Link { get; set; }
    public Color Color { get; set; }

    public MyCourse(string Name,string Link,Color color)
    {
        this.Name = Name;
        this.Link = Link;
        this.Color = color;
    }
}