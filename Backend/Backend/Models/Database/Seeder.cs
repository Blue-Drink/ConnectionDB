namespace Backend.Models.Database;

public class Seeder
{
    public void Seed(DataContext context)
    {
        if (!context.Items.Any())
        {
            var baseNames = new List<string>
            {
                "Mouse", "Key Board", "Speaker", "CPU", "HeadPhone",
                "Desktop", "Desk Lamp", "Table Fan", "File Box", "Key Holder",
                "Books", "Pen Stand", "Land phone", "Mobile phone", "Mac Book",
                "Surface", "UPS"
            };

            foreach (var name in baseNames)
            {
                context.Items.Add(new Item
                {
                    Name = name,
                    ImgUrl = $"https://picsum.photos/200?random={name.GetHashCode()}",
                    Stock = new Random().Next(1, 50)
                });
            }

            int currentCount = baseNames.Count;
            for (int i = currentCount + 1; i <= 30; i++)
            {
                context.Items.Add(new Item
                {
                    Name = $"Extra Gadget {i}",
                    ImgUrl = $"https://picsum.photos/200?random={i}",
                    Stock = new Random().Next(1, 200)
                });
            }

            context.SaveChanges();
        }
    }
}
