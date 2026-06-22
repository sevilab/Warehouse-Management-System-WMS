using System.Collections.Generic;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces
{
    public interface ILocationService
    {
        void AddLocation(Location location);
        void UpdateLocation(int id, string name, string zone, int floor, string description, int capacity);
        void DeleteLocation(int id);
        Location? GetLocationById(int id);
        IEnumerable<Location> GetAllLocations();
        IEnumerable<Location> GetLocationsByZone(string zone);
        IEnumerable<Product> GetProductsInLocation(int locationId);
    }
}
