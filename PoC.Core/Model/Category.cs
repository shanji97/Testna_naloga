namespace PoC.Core.Model;

public sealed class Category
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}