namespace CartService.Application.DTOs;

public class ProductCacheEvent
{
    public List<ProductDto> Products { get; set; } = new();
}

