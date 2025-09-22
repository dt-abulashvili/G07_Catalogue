using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Catalogue.Database.Importer;

internal class DatabaseWriter : IDataWriter<Category>
{
    private readonly string _connectionString;

    internal DatabaseWriter(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = GetCommand(connection, transaction);

        try
        {
            foreach (var category in data)
            {
                AssignParametersToCommand(command, category);
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static SqlCommand GetCommand(SqlConnection connection, SqlTransaction transaction)
    {
        var command = new SqlCommand("ImportProductData_SP", connection);
        command.Transaction = transaction;
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar);
        command.Parameters.Add("@CategoryIsActive", SqlDbType.Bit);
        command.Parameters.Add("@ProductName", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductCode", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductPrice", SqlDbType.Decimal);
        command.Parameters.Add("@ProductIsActive", SqlDbType.Bit);
        return command;
    }

    private static void AssignParametersToCommand(SqlCommand command, Category category)
    {
        foreach (var product in category.Products.Values)
        {
            command.Parameters["@CategoryName"].Value = category.Name;
            command.Parameters["@CategoryIsActive"].Value = category.IsActive;
            command.Parameters["@ProductName"].Value = product.Name;
            command.Parameters["@ProductCode"].Value = product.Code;
            command.Parameters["@ProductPrice"].Value = product.Price;
            command.Parameters["@ProductIsActive"].Value = product.IsActive;
        }
    }
}