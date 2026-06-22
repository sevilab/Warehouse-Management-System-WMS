using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.Services
{
    public class LocationService : ILocationService
    {
        private readonly LocationRepository _locationRepository;
        private readonly ProductRepository _productRepository;

        public LocationService(LocationRepository locationRepository, 
                               ProductRepository productRepository)
        {
            _locationRepository = locationRepository;
            _productRepository = productRepository;
        }

        public void AddLocation(Location location)
        {
            var existing = _locationRepository.GetByName(location.Name);
            if (existing != null)
                throw new InvalidOperationException($"'{location.Name}' isimli lokasyon zaten mevcut.");

            _locationRepository.Add(location);
        }

        public void UpdateLocation(int id, string name, string zone, int floor, string description, int capacity)
        {
            var location = _locationRepository.GetById(id)
                ?? throw new KeyNotFoundException($"Lokasyon bulunamadı. Id: {id}");

            location.Name = name;
            location.Zone = zone;
            location.Floor = floor;
            location.Description = description;
            location.Capacity = capacity;
            _locationRepository.Update(location);
        }

        public void DeleteLocation(int id)
        {
            var productsCount = GetProductsInLocation(id).Count();
            if (productsCount > 0)
                throw new InvalidOperationException("İçinde ürün bulunan bir lokasyon silinemez. Önce ürünleri başka bir yere taşıyın.");

            _locationRepository.Delete(id);
        }

        public Location? GetLocationById(int id)
            => _locationRepository.GetById(id);

        public IEnumerable<Location> GetAllLocations()
            => _locationRepository.GetAllActive();

        public IEnumerable<Location> GetLocationsByZone(string zone)
            => _locationRepository.GetByZone(zone);

        public IEnumerable<Product> GetProductsInLocation(int locationId)
        {
            return _productRepository.GetAllActive()
                .Where(p => p.LocationId == locationId)
                .ToList();
        }
    }
}
