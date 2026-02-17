namespace Backend.Models.Database;

public class Seeder
{
    public void Seed(DataContext context)
    {
        if (context.Items.Count() < 30)
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
                    ImgRoute = "/images/test.png",
                    Stock = new Random().Next(1, 50)
                });
            }

            int currentCount = baseNames.Count;
            for (int i = currentCount + 1; i <= 30; i++)
            {
                context.Items.Add(new Item
                {
                    Name = $"Extra Gadget {i}",
                    ImgRoute = "/images/test.png",
                    Stock = new Random().Next(1, 200)
                });
            }

            context.SaveChanges();
        }
    }
}
