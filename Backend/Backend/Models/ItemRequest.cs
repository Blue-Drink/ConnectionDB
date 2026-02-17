namespace Backend.Models;

public class ItemRequest
{
    public required string Name { get; set; }
    public required int Stock { get; set; }
    public IFormFile? ImageFile { get; set; }
}
