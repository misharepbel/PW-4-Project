using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repositories;

public class Repository : IRepository
{
    private readonly CatalogContext _dataContext;
    public Repository (CatalogContext dataContext) => _dataContext = dataContext;

    public async Task<Category> AddCategoryAsync(Category category)
    {
        _dataContext.Categories.Add(category);
        await _dataContext.SaveChangesAsync();
        return category;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _dataContext.Products.Add(product);
        await _dataContext.SaveChangesAsync();
        return product;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _dataContext.Categories.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _dataContext.Products.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _dataContext.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _dataContext.Products.Update(product);
        await _dataContext.SaveChangesAsync();
        return product;
    }
}
