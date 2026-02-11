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

    public ItemsController(DataContext dataContext)
    {
        _dataContext = dataContext;
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
    public async Task<ActionResult<Item>> PostItem([FromBody] Item newItem)
    {
        if (newItem == null) return BadRequest("Error: Faltan datos");

        _dataContext.Items.Add(newItem);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItems), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(int id, [FromBody] Item item)
    {
        if (id != item.Id)
        {
            return BadRequest("ERROR: producto no encontrado");
        }

        _dataContext.Entry(item).State = EntityState.Modified;

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_dataContext.Items.Any(e => e.Id == id))
            {
                return NotFound("ERROR: producto no encontrado");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
}
