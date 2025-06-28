using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Database.Importer;

public static class DatabaseWriterFactory
{
    public static IDataWriter<Category> Create(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        return new DatabaseWriter(connectionString);
    }
}