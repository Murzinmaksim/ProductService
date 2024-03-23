using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data
{
    public class InventoryServiceContext : DbContext
    {
        public InventoryServiceContext(DbContextOptions<InventoryServiceContext> options) : base(options)
        {
        }

        public DbSet<Inventory> Inventory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>().ToTable("inventory"); 
        }
    }
}
