using Backend.Models.Database;
using Backend.Models.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _environment;
    private readonly string _imagesPath;

    public ItemsController(DataContext dataContext, IWebHostEnvironment environment)
    {
        _dataContext = dataContext;
        _environment = environment;
        _imagesPath = Path.Combine(_environment.WebRootPath, "images");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] ItemQuery queryParams)
    {
        var query = _dataContext.Items.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            query = query.Where(i => i.Name.ToLower().Contains(queryParams.Search));
        }

        query = queryParams.Sort?.ToLower() == "desc"
            ? query.OrderByDescending(i => i.Name)
            : query.OrderBy(i => i.Name);

        return Ok(await query.ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Item>> PostItem([FromForm] ItemRequest request)
    {
        var newItem = new Item
        {
            Name = request.Name,
            ImgRoute = await SaveImageAsync(request.ImageFile) ?? "/images/test.png",
            Stock = request.Stock
        };

        _dataContext.Items.Add(newItem);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItems), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(int id, [FromForm] ItemRequest request)
    {
        var existingItem = await _dataContext.Items.FindAsync(id);
        if (existingItem == null) return NotFound("ERROR: Artículo no encontrado.");

        existingItem.Name = request.Name;
        existingItem.Stock = request.Stock;
        
        if (request.ImageFile != null)
        {
            DeleteImage(existingItem.ImgRoute);
            existingItem.ImgRoute = await SaveImageAsync(request.ImageFile) ?? "/images/test.png";
        }

        await _dataContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _dataContext.Items.FindAsync(id);
        if (item == null) return NotFound("ERROR: producto no encontrado");

        var imagePathToDelete = item.ImgRoute;

        _dataContext.Items.Remove(item);

        try
        {
            await _dataContext.SaveChangesAsync();

            DeleteImage(imagePathToDelete);

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "Error updating database. Image was preserved.");
        }
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!supportedTypes.Contains(extension)) return null;

        if (!Directory.Exists(_imagesPath)) Directory.CreateDirectory(_imagesPath);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_imagesPath, fileName);

        using (var stream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.Read,
            4096,
            useAsync: true))
        {
            await file.CopyToAsync(stream);
        }
        
        return $"/images/{fileName}";
    }

    private void DeleteImage(string imgRoute)
    {
        if (string.IsNullOrEmpty(imgRoute) || imgRoute.Contains("test.png")) return;

        try
        {
            var fileName = Path.GetFileName(imgRoute);
            var fullPath = Path.Combine(_imagesPath, fileName);

            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Orphaned file alert: {ex.Message}");
        }
    }
}
