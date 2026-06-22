using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models
{
    public class Location : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // e.g. "Raf-A-01"

        [StringLength(100)]
        public string Zone { get; set; } = string.Empty; // e.g. "Ön", "Orta", "Arka"

        public int Floor { get; set; } = 1; // 1, 2, 3

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100)]
        public int Capacity { get; set; } = 3; 

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public Location() { }

        public Location(string name, string zone, int floor = 1, string description = "", int capacity = 3)
        {
            Name = name;
            Zone = zone;
            Floor = floor;
            Description = description;
            Capacity = capacity;
        }

        public override string ToString() => $"{Zone} - {Name}";
    }
}
