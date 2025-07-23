using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Models;

namespace StorageManagement.API.Repositories
{
    public class StorageItemRepository : IStorageItemRepository
    {
        private readonly string _connectionString;

        public StorageItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<StorageItem>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<StorageItem>(@"
                SELECT si.*, s.Name as SupplierName 
                FROM StorageItems si 
                LEFT JOIN Suppliers s ON si.SupplierId = s.Id");
        }

        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<StorageItem>(@"
                SELECT si.*, s.Name as SupplierName 
                FROM StorageItems si 
                LEFT JOIN Suppliers s ON si.SupplierId = s.Id 
                WHERE si.Id = @Id", new { Id = id });
        }

        public async Task<StorageItem> CreateAsync(StorageItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO StorageItems (Name, Quantity, Location, Price, SupplierId, PhotoPath) 
                       VALUES (@Name, @Quantity, @Location, @Price, @SupplierId, @PhotoPath);
                       SELECT last_insert_rowid();";
            
            var id = await connection.QuerySingleAsync<int>(sql, item);
            item.Id = id;
            return item;
        }

        public async Task<StorageItem> UpdateAsync(StorageItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            
            // Get existing item
            var existingItem = await connection.QueryFirstOrDefaultAsync<StorageItem>(
                "SELECT * FROM StorageItems WHERE Id = @Id", new { Id = item.Id });
            
            if (existingItem == null)
            {
                throw new InvalidOperationException($"Item with ID {item.Id} not found");
            }
            
            // Use existing values if new values are null or empty
            var updatedName = string.IsNullOrWhiteSpace(item.Name) ? existingItem.Name : item.Name;
            var updatedQuantity = item.Quantity == 0 ? existingItem.Quantity : item.Quantity;
            var updatedLocation = string.IsNullOrWhiteSpace(item.Location) ? existingItem.Location : item.Location;
            var updatedPrice = item.Price ?? existingItem.Price;
            var updatedSupplierId = item.SupplierId ?? existingItem.SupplierId;
            var updatedPhotoPath = item.PhotoPath == "" ? null : item.PhotoPath;
            
            await connection.ExecuteAsync(@"
                UPDATE StorageItems 
                SET Name = @Name, Quantity = @Quantity, Location = @Location, 
                    Price = @Price, SupplierId = @SupplierId, PhotoPath = @PhotoPath, UpdatedAt = CURRENT_TIMESTAMP 
                WHERE Id = @Id", 
                new { 
                    Id = item.Id, 
                    Name = updatedName, 
                    Quantity = updatedQuantity, 
                    Location = updatedLocation,
                    Price = updatedPrice,
                    SupplierId = updatedSupplierId,
                    PhotoPath = updatedPhotoPath
                });
            
            // Return updated item with supplier name
            return await GetByIdAsync(item.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM StorageItems WHERE Id = @Id", new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<StorageItem>> GetBySupplierIdAsync(int supplierId)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<StorageItem>(@"
                SELECT si.*, s.Name as SupplierName 
                FROM StorageItems si 
                LEFT JOIN Suppliers s ON si.SupplierId = s.Id 
                WHERE si.SupplierId = @SupplierId", new { SupplierId = supplierId });
        }
    }
}
