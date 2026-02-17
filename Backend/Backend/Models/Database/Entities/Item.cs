using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Database.Entities;

[Table("Items")]
public class Item
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    public required string ImgRoute { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int Stock { get; set; }
}
