using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence;

namespace CatalogService.Infrastructure.Seeders;

public static class CatalogSeeder
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (!context.Categories.Any())
        {
            var teas = new List<Category>
            {
                new() { Name = "Black Tea" },
                new() { Name = "Green Tea" },
                new() { Name = "Herbal" },
            };

            context.Categories.AddRange(teas);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var firstCategory = context.Categories.First();

            var products = new List<Product>
            {
                new() { Name = "Earl Grey", Ean = "123", SKU = "EG-001", Price = 19.99M, Stock = 50, Country = "UK", CategoryId = firstCategory.Id },
                new() { Name = "Sencha", Ean = "456", SKU = "SE-002", Price = 14.99M, Stock = 30, Country = "Japan", CategoryId = firstCategory.Id },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
