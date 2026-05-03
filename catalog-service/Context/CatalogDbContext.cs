using catalog_service.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace catalog_service.Context
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10,2);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = Guid.Parse("8f7c0a59-7f5b-4dc2-a8fb-6b4a2e8e1c31"),
                    Name = "Ingresso Pista",
                    Description = "Ingresso comum para acesso ao evento.",
                    Price = 120.00m,
                    Category = "Ticket",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = Guid.Parse("2c91d6e4-3bb0-4b61-9b5c-f0b79a1d74e8"),
                    Name = "Ingresso VIP",
                    Description = "Ingresso com acesso a área VIP.",
                    Price = 250.00m,
                    Category = "Ticket",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = Guid.Parse("b57e2f13-91f6-47c9-8f8e-2a0f3d6cbb92"),
                    Name = "Camiseta Oficial",
                    Description = "Camiseta oficial do evento.",
                    Price = 80.00m,
                    Category = "Merchandise",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
