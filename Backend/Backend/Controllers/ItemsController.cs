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
            string searchNormalized = RemoveAccents(queryParams.Search.ToLower());

            result = result.Where(i => RemoveAccents(i.Name.ToLower()).Contains(searchNormalized));
        }

        if (queryParams.Sort!.ToLower() == "desc")
        {
            result = result.OrderByDescending(i => i.Name);
        }
        else
        {
            result = result.OrderBy(i => i.Name);
        }

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
}
