using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseDatabaseApi.Services;

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

    public bool WarehouseExists(int warehouseId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT COUNT(1) FROM  Warehouse WHERE IdWarehouse = @warehouseId",
                connection);
            command.Parameters.AddWithValue("@warehouseId", warehouseId);
            
            connection.Open();
            int count = (int)command.ExecuteScalar();
            connection.Close();

            return count > 0;
        }
    }
    
    public bool IsAmountValid(int amount)
    {
        return amount > 0;
    }

    public bool PurchaseOrderExists(int productId, int amount, DateTime requestCreatedAt)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [Order]
                WHERE IdProduct = @productId
                AND Amount = @amount
                AND CreatedAt < @requestCreatedAt", connection);

            command.Parameters.AddWithValue("@productId", productId);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@requestCreatedAt", requestCreatedAt);
            
            connection.Open();
            int count = (int)command.ExecuteScalar();
            connection.Close();

            return count > 0;
        }
    }

    public bool IsOrderCompleted(int idOrder)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @idOrder", connection);
            command.Parameters.AddWithValue("@idOrder", idOrder);
            
            connection.Open();
            int count = (int)command.ExecuteScalar();
            connection.Close();

            // if count is 0, then no row exists, and the order is not completed.
            return count == 0;
        }
    }

    public bool UpdateOrderFulfilledAt(int idOrder)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand(@"
                UPDATE [Order]
                SET FulfilledAt = @fulfilledAt
                WHERE IdOrder = @idOrder
                AND FulfilledAt IS NULL", connection);

            command.Parameters.AddWithValue("@idOrder", idOrder);
            command.Parameters.AddWithValue("@fulfilledAt", DateTime.Now);
            
            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            // if rows affected is 1, then the row was updated successfully
            return rowsAffected == 1;
        }
    }

    public int? InsertProductWarehouseRecord(int idOrder, int idProduct, int idWarehouse, int amount)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // first we retrieve the price of the product
            var priceCommand = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @idProduct", connection);
            priceCommand.Parameters.AddWithValue("idProduct", idProduct);
            
            connection.Open();
            var priceObject = priceCommand.ExecuteScalar();
            connection.Close();

            if (priceObject == null)
            {
                return null; // product not found or the price is null
            }

            decimal unitPrice = (decimal)priceObject;
            decimal totalPrice = unitPrice * amount;

            var insertCommand = new SqlCommand(@"
                INSERT INTO Product_Warehouse (IdOrder, IdProduct, IdWarehouse, Amount, Price, CreatedAt)
                VALUES (@idOrder, @idProduct, @idWarehouse, @amount, @price, @createdAt);
                SELECT SCOPE_IDENTITY();", connection);
            
            insertCommand.Parameters.AddWithValue("@idOrder", idOrder);
            insertCommand.Parameters.AddWithValue("@idProduct", idProduct);
            insertCommand.Parameters.AddWithValue("@idWarehouse", idWarehouse);
            insertCommand.Parameters.AddWithValue("@amount", amount);
            insertCommand.Parameters.AddWithValue("@price", totalPrice);
            insertCommand.Parameters.AddWithValue("@createdAt", DateTime.Now);

            connection.Open(); 
            var insertedId = insertCommand.ExecuteScalar();
            connection.Close();

            return insertedId != DBNull.Value ? Convert.ToInt32(insertedId) : (int?)null;
        }
    }
    
}