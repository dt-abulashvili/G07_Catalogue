using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Data.Reader;

internal sealed class CsvFileReader : IDataReader<Category>
{
    private readonly string _filePath;

    internal CsvFileReader(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        _filePath = filePath;
    }

    public IEnumerable<Category> GetData()
    {
        var categories = new Dictionary<string, Category>();
        using var reader = new StreamReader(_filePath);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] tokens = line.Split('\t');
            string categoryName = tokens[0];
            string productCode = tokens[3];

            if (!categories.TryGetValue(categoryName, out var category))
            {
                category = CreateCategoryFromTokens(tokens);
                categories[categoryName] = category;
            }
            else
            {
                UpdateCategoryIfChanged(category, tokens);
            }

            if (!category.Products.TryGetValue(productCode, out var product))
            {
                product = CreateProductFromTokens(tokens);
                category.Products[productCode] = product;
            }
            else
            {
                UpdateProductIfChanged(product, tokens);
            }    
        }

        return categories.Values;
    }

    private static Category CreateCategoryFromTokens(string[] tokens)
    {
        return new Category
        {
            Name = tokens[0],
            IsActive = GetBool(tokens[1])
        };
    }

    private static Product CreateProductFromTokens(string[] tokens)
    {
        return new Product
        {
            Name = tokens[2],
            Code = tokens[3],
            Price = decimal.Parse(tokens[4]),
            IsActive = GetBool(tokens[5])
        };
    }

    private static void UpdateCategoryIfChanged(Category category, string[] tokens)
    {
        bool newIsActive = GetBool(tokens[1]);
        if (category.IsActive != newIsActive)
            category.IsActive = newIsActive;
    }
    
    private static void UpdateProductIfChanged(Product product, string[] tokens)
    {
        string newName = tokens[2];
        decimal newPrice = decimal.Parse(tokens[4]);
        bool newIsActive = GetBool(tokens[5]);

        if (product.Name != newName)
            product.Name = newName;

        if (product.Price != newPrice)
            product.Price = newPrice;

        if (product.IsActive != newIsActive)
            product.IsActive = newIsActive;
    }

    private static bool GetBool(string value)
    {
        switch (value)
        {
            case "1": return true;
            case "0": return false;
            default: throw new Exception("Invalid input");
        }
    }
}