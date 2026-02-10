namespace Backend.Models;

public class Item
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string ImgUrl { get; set; }

    public required int Stock { get; set; }
}
