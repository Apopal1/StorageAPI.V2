using Dapper;
using Microsoft.Data.Sqlite;
using StorageManagement.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace StorageManagement.API.Repositories
{
    public class SuperitemRepository : ISuperitemRepository
    {
        private readonly string _connectionString;
        public SuperitemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Superitem> GetAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            var superitems = connection.Query<Superitem>("SELECT * FROM Superitems").ToList();
            foreach (var s in superitems)
            {
                s.SubItems = GetSubItems(s.Id);
            }
            return superitems;
        }

        public Superitem GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var superitem = connection.QueryFirstOrDefault<Superitem>("SELECT * FROM Superitems WHERE Id = @Id", new { Id = id });
            if (superitem != null)
            {
                superitem.SubItems = GetSubItems(id);
            }
            return superitem;
        }

        public int Add(Superitem superitem)
        {
            using var connection = new SqliteConnection(_connectionString);
            var id = connection.ExecuteScalar<int>(@"INSERT INTO Superitems (Name, Location, Quantity) VALUES (@Name, @Location, @Quantity); SELECT last_insert_rowid();", superitem);
            return id;
        }

        public void Update(Superitem superitem)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("UPDATE Superitems SET Name = @Name, Location = @Location, Quantity = @Quantity WHERE Id = @Id", superitem);
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("DELETE FROM Superitems WHERE Id = @Id", new { Id = id });
        }

        public void AddSubItem(int superitemId, int storageItemId, int quantity)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"INSERT OR REPLACE INTO SuperitemStorageItems (SuperitemId, StorageItemId, Quantity) VALUES (@SuperitemId, @StorageItemId, @Quantity)",
                new { SuperitemId = superitemId, StorageItemId = storageItemId, Quantity = quantity });
        }

        public void RemoveSubItem(int superitemId, int storageItemId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("DELETE FROM SuperitemStorageItems WHERE SuperitemId = @SuperitemId AND StorageItemId = @StorageItemId", new { SuperitemId = superitemId, StorageItemId = storageItemId });
        }

        public List<SuperitemSubItemQuantity> GetSubItems(int superitemId)
        {
            using var connection = new SqliteConnection(_connectionString);
            var items = connection.Query<SuperitemSubItemQuantity>(@"
                SELECT si.Id as StorageItemId, si.Name as StorageItemName, ssi.Quantity
                FROM StorageItems si
                INNER JOIN SuperitemStorageItems ssi ON si.Id = ssi.StorageItemId
                WHERE ssi.SuperitemId = @SuperitemId
            ", new { SuperitemId = superitemId }).ToList();
            return items;
        }

        public int GetStorageItemQuantity(int storageItemId)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.ExecuteScalar<int>("SELECT Quantity FROM StorageItems WHERE Id = @Id", new { Id = storageItemId });
        }
    }
}
