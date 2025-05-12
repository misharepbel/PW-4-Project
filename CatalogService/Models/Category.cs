namespace CatalogService.Models;

public class Category : BaseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
