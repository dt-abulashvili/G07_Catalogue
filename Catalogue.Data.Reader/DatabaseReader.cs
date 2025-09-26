using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;

namespace Catalogue.Data.Reader;

internal sealed class DatabaseReader : IDataReader<Category>
{
    private readonly string _connectionString;
    internal DatabaseReader(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        _connectionString = connectionString;
    }

    public IEnumerable<Category> GetData()
    {
        var categories = new Dictionary<string, Category>();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("select * from vw_ProductCatalogue", connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            string categoryName = reader.GetString(0);
            string productCode = reader.GetString(3);

            if (!categories.TryGetValue(categoryName, out var category))
            {
                category = CreateCategoryFromReader(reader);
                categories[categoryName] = category;
            }
            if (!category.Products.TryGetValue(productCode, out var product))
            {
                product = CreateProductFromReader(reader);
                category.Products[productCode] = product;
            }
        }

        return categories.Values;
    }

    private static Category CreateCategoryFromReader(SqlDataReader reader)
    {
        return new Category
        {
            Name = reader.GetString(0),
            IsActive = GetBool(reader.GetInt32(1))
        };
    }

    private static Product CreateProductFromReader(SqlDataReader reader)
    {
        return new Product
        {
            Name = reader.GetString(2),
            Code = reader.GetString(3),
            Price = reader.GetDecimal(4),
            IsActive = GetBool(reader.GetInt32(5))
        };
    }

    private static bool GetBool(int value)
    {
        switch (value)
        {
            case 1: return true;
            case 0: return false;
            default: throw new InvalidDataException($"Unexpected IsActive value: {value}");
        }
    }
}
