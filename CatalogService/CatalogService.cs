using CatalogService.Models;
using CatalogService.Repositories;

namespace CatalogService;

public class CatalogService : ICatalogService
{
    private IRepository _repo;
    public CatalogService(IRepository repo)
    {
        _repo = repo;
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        return await _repo.AddCategoryAsync(category);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        return await _repo.AddProductAsync(product);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _repo.GetAllCategoriesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _repo.GetAllProductsAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        return await _repo.UpdateAsync(product);
    }
}
