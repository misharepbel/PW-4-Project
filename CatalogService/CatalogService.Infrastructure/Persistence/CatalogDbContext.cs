using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence
{
    public class CatalogDbContext : DbContext, ICatalogDbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
    }
}