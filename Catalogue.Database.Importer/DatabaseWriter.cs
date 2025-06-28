using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            foreach (var category in data)
            {
                int? categoryId = GetCategoryIdByName(category.Name, connection);

                if (categoryId == null)
                {
                    using SqlCommand insertCmd = new SqlCommand("InsertCategory", connection);
                    insertCmd.CommandType = CommandType.StoredProcedure;
                    insertCmd.Parameters.AddWithValue("@CategoryName", category.Name);
                    insertCmd.Parameters.AddWithValue("@IsActive", category.IsActive);
                    insertCmd.ExecuteNonQuery();

                    categoryId = GetCategoryIdByName(category.Name, connection);
                }
                else
                {
                    using SqlCommand updateCmd = new SqlCommand("UpdateCategory", connection);
                    updateCmd.CommandType = CommandType.StoredProcedure;
                    updateCmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    updateCmd.Parameters.AddWithValue("@IsActive", category.IsActive);
                    updateCmd.ExecuteNonQuery();
                }

                foreach (var product in category.Products)
                {
                    int? productId = GetProductIdByName(product.Code, connection);

                    if (productId == null)
                    {
                        using SqlCommand insertCmd = new SqlCommand("InsertProduct", connection);
                        insertCmd.CommandType = CommandType.StoredProcedure;
                        insertCmd.Parameters.AddWithValue("@ProductName", product.Name);
                        insertCmd.Parameters.AddWithValue("@ProductCode", product.Code);
                        insertCmd.Parameters.AddWithValue("@Price", product.Price);
                        insertCmd.Parameters.AddWithValue("@IsActive", product.IsActive);
                        insertCmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        insertCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using SqlCommand updateCmd = new SqlCommand("UpdateProduct", connection);
                        updateCmd.CommandType = CommandType.StoredProcedure;
                        updateCmd.Parameters.AddWithValue("@ProductName", product.Name);
                        updateCmd.Parameters.AddWithValue("@ProductCode", product.Code);
                        updateCmd.Parameters.AddWithValue("@Price", product.Price);
                        updateCmd.Parameters.AddWithValue("@IsActive", product.IsActive);
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    private int? GetCategoryIdByName(string categoryName, SqlConnection connection)
    {
        using SqlCommand command = new SqlCommand(
            "select CategoryID from Categories where CategoryName = @Name", connection);
        command.Parameters.AddWithValue("@Name", categoryName);
        object? result = command.ExecuteScalar();

        return result != null ? Convert.ToInt32(result) : null;
    }

    private int? GetProductIdByName(string productCode, SqlConnection connection)
    {
        using SqlCommand command = new SqlCommand(
            "select ProductID from Products where ProductCode = @Code", connection);
        command.Parameters.AddWithValue("@Code", productCode);
        object? result = command.ExecuteScalar();

        return result != null ? Convert.ToInt32(result) : null;
    }
}