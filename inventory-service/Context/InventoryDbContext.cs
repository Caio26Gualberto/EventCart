using inventory_service.Entities;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Context
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(item => item.ProductId)
                .IsUnique();

            modelBuilder.Entity<InventoryItem>().HasData(
                new InventoryItem
                {
                    Id = Guid.Parse("8c7a2bb8-8e0c-4a01-9a91-e5bcf55cf011"),
                    ProductId = Guid.Parse("8f7c0a59-7f5b-4dc2-a8fb-6b4a2e8e1c31"),
                    Quantity = 100,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("efcbf096-7353-4803-84f4-a497c605d581"),
                    ProductId = Guid.Parse("2c91d6e4-3bb0-4b61-9b5c-f0b79a1d74e8"),
                    Quantity = 50,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("d4f6d991-5ef7-4dd4-a2f2-772994d55a5d"),
                    ProductId = Guid.Parse("b57e2f13-91f6-47c9-8f8e-2a0f3d6cbb92"),
                    Quantity = 30,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
