using CatalogService.Models;

namespace CatalogService;

public interface ICatalogService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Product> GetByIdAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task<Category> AddCategoryAsync(Category category);
    Task<Product> UpdateAsync(Product product);
}
