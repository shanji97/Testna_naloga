using PoC.Core.Contracts;
using PoC.Core.Repository;

namespace PoC.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet(string.Empty, GetProducts)
            .WithName("GetProducts")
            .WithSummary("Retrieves products using optional filter criteria.")
            .WithDescription("Filters products by name, category id, minimum price, and maximum price.");

        group.MapPatch("/{id:int:min(1)}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Updates a product.")
            .WithDescription("Updates the product name and/or price for the specified product id.");

        return app;
    }

    private static IResult GetProducts(
        [AsParameters] ProductFilter productFilter,
        IProductRepository repository)
    {
        if (productFilter.MinPrice is < 0)
        {
            return Results.BadRequest("Minimum price cannot be negative.");
        }

        if (productFilter.MaxPrice is < 0)
        {
            return Results.BadRequest("Maximum price cannot be negative.");
        }

        if (productFilter.MinPrice > productFilter.MaxPrice)
        {
            return Results.BadRequest("Minimum price cannot be greater than maximum price.");
        }

        var products = repository.GetFiltered(productFilter);

        return Results.Ok(products);
    }

    private static IResult UpdateProduct(
        int id,
        UpdateProductRequest request,
        IProductRepository repository)
    {
        if (id <= 0)
        {
            return Results.BadRequest("Product id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) && request.Price is null)
        {
            return Results.BadRequest("At least one field must be provided: name or price.");
        }

        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Product name cannot be empty.");
        }

        if (request.Price is < 0)
        {
            return Results.BadRequest("Product price cannot be negative.");
        }

        var updatedProduct = repository.Update(id, request);

        return updatedProduct is null
            ? Results.NotFound($"Product with id {id} was not found.")
            : Results.Ok(updatedProduct);
    }
}