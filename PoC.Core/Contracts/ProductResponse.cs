using PoC.Core.Model;

namespace PoC.Core.Contracts;

public sealed class ProductResponse
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public required Category Category { get; init; }
}
