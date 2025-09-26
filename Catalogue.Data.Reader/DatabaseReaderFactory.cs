using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Data.Reader;

public static class DatabaseReaderFactory
{
    public static IDataReader<Category> Create(string connectionString)
        => new DatabaseReader(connectionString);
}
