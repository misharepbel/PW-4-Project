using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces;

public interface ICatalogRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> AddProductAsync(Product product);
    Task<Category> AddCategoryAsync(Category category);
    Task<Product> UpdateAsync(Product product);
}
