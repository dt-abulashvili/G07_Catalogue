using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Data.Reader;

public static class CsvFileReaderFactory
{
    public static IDataReader<Category> Create(string filePath)
        => new CsvFileReader(filePath);
}