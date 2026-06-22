using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{    //TEMEL SINIF 
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }
        // Şablon: Silme işleminin adımları sabittir
        //(Bul - Silindi İşaretle - Kaydet)
        public void Delete(int id)
        {
            var entity = GetById(id)
                ?? throw new KeyNotFoundException($"Id: {id} bulunamadı.");
            entity.MarkAsDeleted();
            _context.SaveChanges();
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public IEnumerable<T> GetAllActive()
        {
            return _dbSet.Where(e => !e.IsDeleted).ToList();
        }
        public T? GetById(int id)
        {
            return _dbSet.FirstOrDefault(e => e.Id == id && !e.IsDeleted);
        }//BaseRepository<T>
        // Boş bırakılan adım — her alt sınıf kendi güncelleme mantığını yazar
        public abstract void Update(T entity);
    }
}