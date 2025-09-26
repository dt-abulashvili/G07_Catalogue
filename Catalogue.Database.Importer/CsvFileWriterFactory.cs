using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Database.Importer;

public static class CsvFileWriterFactory
{
    public static IDataWriter<Category> Create(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        return new CsvFileWriter(filePath);
    }
}
