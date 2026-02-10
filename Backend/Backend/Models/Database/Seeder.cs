namespace Backend.Models.Database;

public class Seeder
{
    public void Seed(DataContext context)
    {
        if (!context.Items.Any())
        {
            for (int i = 1; i <= 30; i++)
            {
                context.Items.Add(new Item()
                {
                    Name = $"Artículo {i}",
                    ImgUrl = $"https://picsum.photos/200?random={i}",
                    Stock = new Random().Next(1, 100)
                });
            }

            context.SaveChanges();
        }
    }
}
