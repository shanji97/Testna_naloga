namespace PoC.Core.Model;

public sealed class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }
}