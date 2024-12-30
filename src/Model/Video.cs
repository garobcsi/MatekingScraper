namespace MatekingScraper.Model;

public class Video
{
    public string? Name { get; set; } = null;
    public string? Link { get; set; } = null;
    public uint? Number { get; set; } = null;
    public bool Accessible { get; set; } = false;
}