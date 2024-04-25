namespace WarehouseDatabaseApi.WarehouseDatabaseApi.Service.Application.Services;
using System;
using System.Data;
using System.Data.SqlClient;

public class WarehouseService
{
    private readonly string _connectionString;

    public WarehouseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool ProductExists(int productId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT COUNT(1) FROM Product WHERE IdProduct = @productId", connection);
            command.Parameters.AddWithValue("@productId", productId);
            
            connection.Open();
            int count = (int)command.ExecuteScalar();
            connection.Close();

            return count > 0;
        }
    }


    
}