namespace Catalogue.Shared.Models
{
    public sealed class Category
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public Dictionary<string, Product> Products { get; set; } = new Dictionary<string, Product>();

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Category other)
                return GetHashCode().Equals(other.GetHashCode());
            return false;
        }
    }
}
