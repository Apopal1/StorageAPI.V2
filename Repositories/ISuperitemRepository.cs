using StorageManagement.API.Models;
using System.Collections.Generic;

namespace StorageManagement.API.Repositories
{
    public interface ISuperitemRepository
    {
        IEnumerable<Superitem> GetAll();
        Superitem GetById(int id);
        int Add(Superitem superitem);
        void Update(Superitem superitem);
        void Delete(int id);
        void AddSubItem(int superitemId, int storageItemId, int quantity);
        void RemoveSubItem(int superitemId, int storageItemId);
        List<SuperitemSubItemQuantity> GetSubItems(int superitemId);
        int GetStorageItemQuantity(int storageItemId);
    }
}
