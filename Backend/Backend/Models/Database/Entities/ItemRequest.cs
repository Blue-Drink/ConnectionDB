using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Backend.Models.Database.Entities;

public class ItemRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int Stock { get; set; }

    [Required]
    public IFormFile? ImageFile { get; set; }
}
