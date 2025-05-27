using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
