namespace Catalogue.Shared.Models;

public sealed class Category
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public IDictionary<string, Product> Products { get; } = new Dictionary<string, Product>();
}