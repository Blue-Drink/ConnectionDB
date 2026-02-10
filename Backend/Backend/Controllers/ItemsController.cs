using Backend.Models;
using Backend.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController
{
    private readonly DataContext _dataContext;

    public ItemsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public IEnumerable<Item> GetItems()
    {
        return _dataContext.Items;
    }
}
