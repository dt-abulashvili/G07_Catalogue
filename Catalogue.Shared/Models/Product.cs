namespace Catalogue.Shared.Models;

public sealed class Product
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}