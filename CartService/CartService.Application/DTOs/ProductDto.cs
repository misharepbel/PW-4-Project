namespace CartService.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ean { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Country { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}

