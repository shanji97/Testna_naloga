namespace PoC.Core.Contracts;

public sealed class ProductFilter
{
    public string? Name { get; init; }

    public int? CategoryId { get; init; }

    //public string? CategoryName { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }
}