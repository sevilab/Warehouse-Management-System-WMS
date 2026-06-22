using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public override void Update(User entity)
        {
            _context.SaveChanges();
        }

        public User? GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(u =>
                u.Username.ToLower() == username.ToLower() &&
                !u.IsDeleted && u.IsActive);
        }

        public bool UsernameExists(string username)
        {
            return _dbSet.Any(u =>
                u.Username.ToLower() == username.ToLower() &&
                !u.IsDeleted);
        }
    }
}