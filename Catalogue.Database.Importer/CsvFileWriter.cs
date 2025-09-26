using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Database.Importer;

internal class CsvFileWriter : IDataWriter<Category>
{
    private readonly string _filePath;
    internal CsvFileWriter(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }
    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var writer = new StreamWriter(_filePath);
        foreach (var category in data)
        {
            foreach (var product in category.Products.Values)
            {
                string line = $"{category.Name}\t{(category.IsActive ? 1 : 0)}\t{product.Name}\t{product.Code}\t{product.Price}\t{(product.IsActive ? 1 : 0)}";
                writer.WriteLine(line);
            }
        }
    }
}
