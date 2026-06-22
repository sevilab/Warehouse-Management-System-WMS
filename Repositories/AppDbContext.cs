using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;
using System.IO;

namespace WarehouseManagementSystem.Repositories
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "warehouse.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.SKU).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Category).HasConversion<string>();
                
                // Location relationship
                entity.HasOne<Location>()
                      .WithMany(l => l.Products)
                      .HasForeignKey(p => p.LocationId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Email).HasMaxLength(100);
                entity.Property(s => s.Phone).HasMaxLength(20);
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(t => t.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(t => t.Type).HasConversion<string>();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Role).HasConversion<string>();
            });

            // Location
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Name).IsRequired().HasMaxLength(50);
                entity.Property(l => l.Zone).HasMaxLength(100);
            });

            // Return
            modelBuilder.Entity<Return>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(r => r.SKU).IsRequired().HasMaxLength(50);
                entity.Property(r => r.Reason).HasConversion<string>();
                entity.Property(r => r.Action).HasConversion<string>();
            });

            // PurchaseOrder (Phase 10)
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(o => o.SupplierName).IsRequired().HasMaxLength(200);
                entity.Property(o => o.Status); // Use default (int) to match SQLite INTEGER
                entity.Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
}