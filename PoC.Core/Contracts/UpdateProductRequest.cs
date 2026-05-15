namespace PoC.Core.Contracts;

public sealed record UpdateProductRequest(
    string? Name,
    decimal? Price);