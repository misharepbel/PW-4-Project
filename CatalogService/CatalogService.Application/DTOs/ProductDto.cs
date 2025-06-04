namespace CatalogService.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ean { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Country { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public string CategoryName { get; set; } = string.Empty;
}
