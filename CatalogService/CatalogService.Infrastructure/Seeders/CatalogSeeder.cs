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
                new() { Id = Guid.NewGuid(), Name = "Earl Grey", Ean = "123", SKU = "EG-001", Price = 19.99M, Stock = 50, Country = "UK", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Sencha", Ean = "456", SKU = "SE-002", Price = 14.99M, Stock = 30, Country = "Japan", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Chamomile", Ean = "789", SKU = "CH-003", Price = 9.99M, Stock = 40, Country = "Poland", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Assam", Ean = "101", SKU = "AS-004", Price = 11.99M, Stock = 45, Country = "India", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Matcha", Ean = "102", SKU = "MA-005", Price = 21.99M, Stock = 20, Country = "Japan", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Oolong", Ean = "103", SKU = "OO-006", Price = 17.99M, Stock = 35, Country = "China", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Peppermint", Ean = "104", SKU = "PP-007", Price = 8.99M, Stock = 25, Country = "USA", CategoryId = firstCategory.Id },
                new() { Id = Guid.NewGuid(), Name = "Darjeeling", Ean = "105", SKU = "DA-008", Price = 18.99M, Stock = 55, Country = "India", CategoryId = firstCategory.Id }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
