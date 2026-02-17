using Backend.Models;
using Backend.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _environment;

    public ItemsController(DataContext dataContext, IWebHostEnvironment environment)
    {
        _dataContext = dataContext;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] ItemQuery queryParams)
    {
        var items = await _dataContext.Items.ToListAsync();
        var result = items.AsEnumerable();

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            string cleanSearch = System.Text.RegularExpressions.Regex.Replace(queryParams.Search, @"[^\w\s]", "");
            string searchNormalized = RemoveAccents(cleanSearch.ToLower());

            result = result.Where(i => RemoveAccents(i.Name.ToLower()).Contains(searchNormalized));
        }

        result = queryParams.Sort?.ToLower() == "desc"
            ? result.OrderByDescending(i => i.Name)
            : result.OrderBy(i => i.Name);

        return Ok(result.ToList());
    }

    private string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        return new string(text
            .Normalize(System.Text.NormalizationForm.FormD)
            .Where(ch => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<Item>> PostItem([FromForm] ItemRequest request)
    {
        string imgRoute = "/images/test.png";

        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
            var uploadPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream);
            }
            imgRoute = $"/images/{fileName}";
        }

        var newItem = new Item
        {
            Name = request.Name,
            Stock = request.Stock,
            ImgRoute = imgRoute
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
        
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream);
            }
            existingItem.ImgRoute = $"/images/{fileName}";
        }

        await _dataContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _dataContext.Items.FindAsync(id);

        if (item == null)
        {
            return NotFound("ERROR: producto no encontrado");
        }

        _dataContext.Items.Remove(item);

        await _dataContext.SaveChangesAsync();

        return NoContent();
    }
}
