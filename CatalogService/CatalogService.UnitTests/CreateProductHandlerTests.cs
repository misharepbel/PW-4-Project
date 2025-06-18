using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Mappings;
using CatalogService.Application.Products.Commands;
using CatalogService.Application.Products.Handlers;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CatalogService.UnitTests;

public class CreateProductHandlerTests
{
    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
        return cfg.CreateMapper();
    }

    private static CatalogDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CatalogDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProductAndPublishesEvent()
    {
        var context = CreateContext();
        var category = new Category { Id = 1, Name = "Tea" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var products = new List<Product>();
        var repo = new Mock<ICatalogRepository>();
        repo.Setup(r => r.AddProductAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => { products.Add(p); return p; });
        repo.Setup(r => r.GetAllProductsAsync())
            .ReturnsAsync(products);

        var producer = new Mock<IProductCacheProducer>();
        var mapper = CreateMapper();
        var handler = new CreateProductHandler(repo.Object, mapper, context, producer.Object);

        var command = new CreateProductCommand("Earl Grey", "ean", 1m, 2, "UK", "sku", category.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Earl Grey", result.Name);
        Assert.Equal(category.Name, result.CategoryName);
        repo.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Once);
        producer.Verify(p => p.PublishAsync(It.IsAny<ProductCacheEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MissingCategory_ThrowsArgumentException()
    {
        var context = CreateContext();
        var repo = new Mock<ICatalogRepository>();
        var producer = new Mock<IProductCacheProducer>();
        var mapper = CreateMapper();
        var handler = new CreateProductHandler(repo.Object, mapper, context, producer.Object);

        var command = new CreateProductCommand("Test", "ean", 1m, 2, "UK", "sku", 99);
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        repo.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Never);
    }
}
