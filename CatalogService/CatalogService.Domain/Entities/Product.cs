namespace CatalogService.Domain.Entities;

public class Product : BaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ean { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Country { get; set; } = default!;
    public string SKU { get; set; } = default!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}
