using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace Catalogue.Database.Importer;

internal class DatabaseWriter : IDataWriter<Category>
{
    private readonly string _connectionString;

    public DatabaseWriter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        using SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            foreach (var category in data)
            {
                UpdateCategoryAndProducts(category, connection, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private void UpdateCategoryAndProducts(Category category, SqlConnection connection, SqlTransaction transaction)
    {
        foreach (var product in category.Products.Values)
        {
            using SqlCommand command = new SqlCommand("ImportProductData_SP", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CategoryName", category.Name);
            command.Parameters.AddWithValue("@CategoryIsActive", category.IsActive);
            command.Parameters.AddWithValue("@ProductName", product.Name);
            command.Parameters.AddWithValue("@ProductCode", product.Code);
            command.Parameters.AddWithValue("@ProductPrice", product.Price);
            command.Parameters.AddWithValue("@ProductIsActive", product.IsActive);

            command.ExecuteNonQuery();
        }
    }  
}