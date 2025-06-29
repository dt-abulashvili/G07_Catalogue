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
                int? categoryId = GetCategoryIdByName(category.Name, connection, transaction);

                if (categoryId == null)
                {
                    categoryId = InsertCategory(category, connection, transaction);
                }
                else if (IsCategoryChanged(categoryId.Value, category, connection, transaction))
                {
                    UpdateCategory(categoryId.Value, category, connection, transaction);
                }

                foreach (var product in category.Products.Values)
                {
                    int? productId = GetProductIdByName(product.Code, connection, transaction);

                    if (productId == null)
                    {
                        InsertProduct(product, categoryId.Value, connection, transaction);
                    }
                    else if (IsProductChanged(product.Code, product, connection, transaction))
                    {
                        UpdateProduct(product, connection, transaction);
                    }
                }
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private int? GetCategoryIdByName(string categoryName, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand command = new SqlCommand(
            "select CategoryID from Categories where CategoryName = @Name", connection, transaction);
        command.Parameters.AddWithValue("@Name", categoryName);
        object? result = command.ExecuteScalar();

        return result != null ? Convert.ToInt32(result) : null;
    }

    private int? GetProductIdByName(string productCode, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand command = new SqlCommand(
            "select ProductID from Products where ProductCode = @Code", connection, transaction);
        command.Parameters.AddWithValue("@Code", productCode);
        object? result = command.ExecuteScalar();

        return result != null ? Convert.ToInt32(result) : null;
    }

    private int InsertCategory(Category category, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand cmd = new SqlCommand("InsertCategory", connection, transaction);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryName", category.Name);
        cmd.Parameters.AddWithValue("@IsActive", category.IsActive);
        cmd.ExecuteNonQuery();

        return GetCategoryIdByName(category.Name, connection, transaction) ?? throw new Exception("Failed to retrieve inserted category ID.");
    }

    private void UpdateCategory(int categoryId, Category category, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand cmd = new SqlCommand("UpdateCategory", connection, transaction);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
        cmd.Parameters.AddWithValue("@IsActive", category.IsActive);
        cmd.ExecuteNonQuery();
    }

    private void InsertProduct(Product product, int categoryId, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand cmd = new SqlCommand("InsertProduct", connection, transaction);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
        cmd.Parameters.AddWithValue("@ProductCode", product.Code);
        cmd.Parameters.AddWithValue("@ProductName", product.Name);
        cmd.Parameters.AddWithValue("@Price", product.Price);
        cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
        cmd.ExecuteNonQuery();
    }

    private void UpdateProduct(Product product, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand cmd = new SqlCommand("UpdateProduct", connection, transaction);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ProductCode", product.Code);
        cmd.Parameters.AddWithValue("@ProductName", product.Name);
        cmd.Parameters.AddWithValue("@Price", product.Price);
        cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
        cmd.ExecuteNonQuery();
    }

    private bool IsCategoryChanged(int categoryId, Category category, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand command = new SqlCommand(
            "select IsActive from Categories where CategoryID = @ID", connection, transaction);
        command.Parameters.AddWithValue("@ID", categoryId);

        using SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            bool dbIsActive = reader.GetBoolean(0);
            return dbIsActive != category.IsActive;
        }

        return true; //issue here
    }

    private bool IsProductChanged(string productCode, Product product, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand command = new SqlCommand(
            "select ProductName, Price, IsActive from Products where ProductCode = @Code", connection, transaction);
        command.Parameters.AddWithValue("@Code", productCode);

        using SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            string dbName = reader.GetString(0);
            decimal dbPrice = reader.GetDecimal(1);
            bool dbIsActive = reader.GetBoolean(2);

            return dbName != product.Name || dbPrice != product.Price || dbIsActive != product.IsActive;
        }

        return true; //issue here
    }
}