using PoC.Core.Contracts;
using PoC.Core.Repository;

namespace PoC.Tests.Repository;

public sealed class InMemoryRepositoryTests
{
    [Fact]
    public void GetFiltered_WhenNameIsProvided_ReturnsMatchingProducts()
    {
        var repository = new InMemoryRepository();

        var filter = new ProductFilter
        {
            Name = "lap"
        };

        var products = repository.GetFiltered(filter);

        Assert.Single(products);
        Assert.Equal("Laptop", products.First().Name);
    }

    [Fact]
    public void GetFiltered_WhenCategoryIdIsProvided_ReturnsProductsFromCategory()
    {
        var repository = new InMemoryRepository();

        var filter = new ProductFilter
        {
            CategoryId = 1
        };

        var products = repository.GetFiltered(filter);

        Assert.Equal(2, products.Count);
        Assert.All(products, product => Assert.Equal(1, product.CategoryId));
    }

    [Fact]
    public void GetFiltered_WhenPriceRangeIsProvided_ReturnsProductsWithinRange()
    {
        var repository = new InMemoryRepository();

        var filter = new ProductFilter
        {
            MinPrice = 10m,
            MaxPrice = 100m
        };

        var products = repository.GetFiltered(filter);

        Assert.Equal(3, products.Count);

        Assert.All(products, product =>
        {
            Assert.True(product.Price >= 10m);
            Assert.True(product.Price <= 100m);
        });
    }

    [Fact]
    public void Update_WhenProductExists_UpdatesNameAndPrice()
    {
        var repository = new InMemoryRepository();

        var request = new UpdateProductRequest(
            Name: "Updated Laptop",
            Price: 999.99m);

        var product = repository.Update(1, request);

        Assert.NotNull(product);
        Assert.Equal("Updated Laptop", product.Name);
        Assert.Equal(999.99m, product.Price);
    }

    [Fact]
    public void Update_WhenOnlyNameIsProvided_UpdatesOnlyName()
    {
        var repository = new InMemoryRepository();

        var originalProduct = repository.GetFiltered(new ProductFilter())
            .First(product => product.Id == 2);

        var originalPrice = originalProduct.Price;

        var request = new UpdateProductRequest(
            Name: "Mechanical Keyboard",
            Price: null);

        var product = repository.Update(2, request);

        Assert.NotNull(product);
        Assert.Equal("Mechanical Keyboard", product.Name);
        Assert.Equal(originalPrice, product.Price);
    }

    [Fact]
    public void Update_WhenOnlyPriceIsProvided_UpdatesOnlyPrice()
    {
        var repository = new InMemoryRepository();

        var originalProduct = repository.GetFiltered(new ProductFilter())
            .First(product => product.Id == 3);

        var originalName = originalProduct.Name;

        var request = new UpdateProductRequest(
            Name: null,
            Price: 34.99m);

        var product = repository.Update(3, request);

        Assert.NotNull(product);
        Assert.Equal(originalName, product.Name);
        Assert.Equal(34.99m, product.Price);
    }

    [Fact]
    public void Update_WhenProductDoesNotExist_ReturnsNull()
    {
        var repository = new InMemoryRepository();

        var request = new UpdateProductRequest(
            Name: "Missing Product",
            Price: 10m);

        var product = repository.Update(999, request);

        Assert.Null(product);
    }
}