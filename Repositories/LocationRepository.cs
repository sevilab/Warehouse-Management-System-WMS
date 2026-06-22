using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class LocationRepository : BaseRepository<Location>
    {
        public LocationRepository(AppDbContext context) : base(context)
        {
        }

        public override void Update(Location entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                existing.Name = entity.Name;
                existing.Zone = entity.Zone;
                existing.Description = entity.Description;
                _context.SaveChanges();
            }
        }

        public Location? GetByName(string name)
        {
            return _dbSet.FirstOrDefault(l => l.Name == name && !l.IsDeleted);
        }

        public IEnumerable<Location> GetByZone(string zone)
        {
            return _dbSet.Where(l => l.Zone == zone && !l.IsDeleted).ToList();
        }
    }
}
