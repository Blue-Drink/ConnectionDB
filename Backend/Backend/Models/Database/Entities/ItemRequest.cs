using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Database.Entities;

public class ItemRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int Stock { get; set; }

    public IFormFile? ImageFile { get; set; }
}
