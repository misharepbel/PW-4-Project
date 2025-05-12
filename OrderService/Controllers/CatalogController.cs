using CatalogService.Models;
using CatalogService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private ICatalogService _ps;
    public CatalogController(ICatalogService ps)
    {
        _ps = ps;
    }

    // GET: api/teas
    [HttpGet("api/teas")]
    public async Task<ActionResult> GetProducts()
    {
        var result = await _ps.GetAllProductsAsync();
        return Ok(result);
    }

    // GET: api/categories
    [HttpGet("api/categories")]
    public async Task<ActionResult> GetCategories()
    {
        var result = await _ps.GetAllCategoriesAsync();
        return Ok(result);
    }

    // GET api/teas/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduct(int id)
    {
        var result = await _ps.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    // POST api/teas
    [HttpPost("teas")]
    public async Task<ActionResult> PostProduct([FromBody] Product product)
    {
        var result = await _ps.AddProductAsync(product);
        return Ok(result);
    }

    // POST api/categories
    [HttpPost("categories")]
    public async Task<ActionResult> PostCategory([FromBody] Category category)
    {
        var result = await _ps.AddCategoryAsync(category);
        return Ok(result);
    }

    // PUT api/teas/{id}
    [HttpPut("teas/{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Product product)
    {
        var result = await _ps.UpdateAsync(product);
        return Ok(result);
    }

    // DELETE api/teas/{id}
    [HttpDelete("teas/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var product = await _ps.GetByIdAsync(id);
        product.Deleted = true;
        var result = await _ps.UpdateAsync(product);
        return Ok(result);
    }
}
