using Backend.Models.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Database;

public class Seeder
{
    public void Seed(DataContext _dataContext)
    {
        var random = new Random();
        var baseNames = new List<string>
        {
            "Mouse", "Key Board", "Speaker", "CPU", "HeadPhone",
            "Desktop", "Desk Lamp", "Table Fan", "File Box", "Key Holder",
            "Books", "Pen Stand", "Land phone", "Mobile phone", "Mac Book",
            "Surface", "UPS"
        };

        var itemsToSeed = new List<Item>();

        foreach (var name in baseNames)
        {
            itemsToSeed.Add(new Item
            {
                Name = name,
                ImgRoute = "/images/test.png",
                Stock = random.Next(1, 50)
            });
        }

        int currentCount = _dataContext.Items.Count() + baseNames.Count();
        for (int i = currentCount + 1; i <= 30; i++)
        {
            itemsToSeed.Add(new Item
            {
                Name = $"Extra Gadget {i}",
                ImgRoute = "/images/test.png",
                Stock = new Random().Next(1, 200)
            });
        }

        _dataContext.Items.AddRange(itemsToSeed);
        _dataContext.SaveChanges();
    }
}
