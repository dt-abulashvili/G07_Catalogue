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
                    categoryId = InsertCategory(category, connection);
                }
                else
                {
                    UpdateCategory(categoryId.Value, category, connection);
                }

                foreach (var product in category.Products.Values)
                {
                    int? productId = GetProductIdByName(product.Code, connection);

                    if (productId == null)
                    {
                        InsertProduct(product, categoryId.Value, connection);
                    }
                    else
                    {
                        UpdateProduct(product, connection);
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

    private int InsertCategory(Category category, SqlConnection connection)
    {
        using SqlCommand cmd = new SqlCommand("InsertCategory", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryName", category.Name);
        cmd.Parameters.AddWithValue("@IsActive", category.IsActive);
        cmd.ExecuteNonQuery();

        return GetCategoryIdByName(category.Name, connection) ?? throw new Exception("Failed to retrieve inserted category ID.");
    }

    private void UpdateCategory(int categoryId, Category category, SqlConnection connection)
    {
        using SqlCommand cmd = new SqlCommand("UpdateCategory", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
        cmd.Parameters.AddWithValue("@IsActive", category.IsActive);
        cmd.ExecuteNonQuery();
    }

    private void InsertProduct(Product product, int categoryId, SqlConnection connection)
    {
        using SqlCommand cmd = new SqlCommand("InsertProduct", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
        cmd.Parameters.AddWithValue("@ProductCode", product.Code);
        cmd.Parameters.AddWithValue("@ProductName", product.Name);
        cmd.Parameters.AddWithValue("@Price", product.Price);
        cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
        cmd.ExecuteNonQuery();
    }

    private void UpdateProduct(Product product, SqlConnection connection)
    {
        using SqlCommand cmd = new SqlCommand("UpdateProduct", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ProductCode", product.Code);
        cmd.Parameters.AddWithValue("@ProductName", product.Name);
        cmd.Parameters.AddWithValue("@Price", product.Price);
        cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
        cmd.ExecuteNonQuery();
    }

}