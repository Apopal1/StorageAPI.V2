using Dapper;
using Microsoft.Data.Sqlite;
using StorageManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageManagement.API.Repositories
{
    public class OutgoingOrderRepository : IOutgoingOrderRepository
    {
        private readonly string _connectionString;

        public OutgoingOrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<OutgoingOrder>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<OutgoingOrder>("SELECT * FROM OutgoingOrders");
        }

        public async Task<OutgoingOrder> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<OutgoingOrder>("SELECT * FROM OutgoingOrders WHERE Id = @Id", new { Id = id });
        }

        public async Task<OutgoingOrder> CreateAsync(OutgoingOrder order)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO OutgoingOrders (Recipient, SerialNumber, Status) 
                        VALUES (@Recipient, @SerialNumber, @Status);
                        SELECT last_insert_rowid();";
            var id = await connection.ExecuteScalarAsync<int>(sql, order);
            order.Id = id;
            return order;
        }

        public async Task UpdateAsync(OutgoingOrder order)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "UPDATE OutgoingOrders SET Recipient = @Recipient, SerialNumber = @SerialNumber, Status = @Status WHERE Id = @Id";
            await connection.ExecuteAsync(sql, order);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.ExecuteAsync("DELETE FROM OutgoingOrders WHERE Id = @Id", new { Id = id });
        }

        public async Task AddOrderItemAsync(OutgoingOrderItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "INSERT INTO OutgoingOrderItems (OutgoingOrderId, StorageItemId, Quantity, CustomItemDescription) VALUES (@OutgoingOrderId, @StorageItemId, @Quantity, @CustomItemDescription)";
            await connection.ExecuteAsync(sql, item);
        }

        public async Task<IEnumerable<OutgoingOrderItem>> GetOrderItemsAsync(int orderId)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<OutgoingOrderItem>("SELECT * FROM OutgoingOrderItems WHERE OutgoingOrderId = @OrderId", new { OrderId = orderId });
        }
    }
}