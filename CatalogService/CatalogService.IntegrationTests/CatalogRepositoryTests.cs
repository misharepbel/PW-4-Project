using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests;

public class CatalogRepositoryTests
{
    private static CatalogDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CatalogDbContext(options);
    }

    [Fact]
    public async Task AddProductAndRetrieveById_Works()
    {
        using var context = CreateContext();
        var repo = new CatalogRepository(context);
        var category = new Category { Name = "Black" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Earl",
            Ean = "1",
            SKU = "T1",
            Price = 1m,
            Stock = 1,
            Country = "UK",
            CategoryId = category.Id,
            Category = category
        };
        await repo.AddProductAsync(product);

        var loaded = await repo.GetByIdAsync(product.Id);
        Assert.NotNull(loaded);
        Assert.Equal("Earl", loaded!.Name);
        Assert.NotNull(loaded.Category);
    }

    [Fact]
    public async Task GetAllProducts_ExcludesDeleted()
    {
        using var context = CreateContext();
        var repo = new CatalogRepository(context);
        var category = new Category { Name = "Herbal" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        context.Products.AddRange(
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mint",
                Ean = "2",
                SKU = "T2",
                Price = 1m,
                Stock = 1,
                Country = "UK",
                CategoryId = category.Id,
                Category = category
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Old",
                Ean = "3",
                SKU = "T3",
                Price = 1m,
                Stock = 1,
                Country = "UK",
                Deleted = true,
                CategoryId = category.Id,
                Category = category
            }
        );
        await context.SaveChangesAsync();

        var all = await repo.GetAllProductsAsync();
        Assert.Single(all);
        Assert.Equal("Mint", all.First().Name);
    }
}
