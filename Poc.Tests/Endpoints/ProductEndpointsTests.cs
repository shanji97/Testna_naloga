using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PoC.Core.Contracts;
using PoC.Core.Model;
using PoC.Core.Repository;

namespace PoC.Tests.Endpoints;

public sealed class ProductEndpointsTests
{
    [Fact]
    public async Task GetProducts_WhenUsingFakeRepository_ReturnsProducts()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<Product>>(
            "/api/products?name=test",
            cancellationToken);

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("Test Product", products[0].Name);
    }

    [Fact]
    public async Task GetProducts_WhenMinPriceIsNegative_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/products?minPrice=-1",
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WhenMaxPriceIsNegative_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/products?maxPrice=-1",
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WhenMinPriceIsGreaterThanMaxPrice_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/products?minPrice=100&maxPrice=10",
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductExists_ReturnsUpdatedProduct()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest(
            Name: "Updated Test Product",
            Price: 15.99m);

        var response = await client.PatchAsJsonAsync(
            "/api/products/1",
            request,
            cancellationToken);

        var product = await response.Content.ReadFromJsonAsync<Product>(
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(product);
        Assert.Equal("Updated Test Product", product.Name);
        Assert.Equal(15.99m, product.Price);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest(
            Name: "Missing Product",
            Price: 20m);

        var response = await client.PatchAsJsonAsync(
            "/api/products/999",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenRequestIsEmpty_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest(
            Name: null,
            Price: null);

        var response = await client.PatchAsJsonAsync(
            "/api/products/1",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenNameIsEmpty_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest(
            Name: string.Empty,
            Price: null);

        var response = await client.PatchAsJsonAsync(
            "/api/products/1",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenPriceIsNegative_ReturnsBadRequest()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactoryWithFakeRepository();
        using var client = factory.CreateClient();

        var request = new UpdateProductRequest(
            Name: null,
            Price: -5m);

        var response = await client.PatchAsJsonAsync(
            "/api/products/1",
            request,
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactoryWithFakeRepository()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IProductRepository>();
                    services.AddSingleton<IProductRepository, FakeProductRepository>();
                });
            });
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products =
        [
            new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.99m,
                CategoryId = 1
            }
        ];

        public IReadOnlyCollection<Product> GetFiltered(ProductFilter productFilter)
        {
            return _products;
        }

        public Product? Update(int id, UpdateProductRequest request)
        {
            var product = _products.FirstOrDefault(product => product.Id == id);

            if (product is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                product.Name = request.Name.Trim();
            }

            if (request.Price is not null)
            {
                product.Price = request.Price.Value;
            }

            return product;
        }
    }
}