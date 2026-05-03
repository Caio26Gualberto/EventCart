using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace catalog_service.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("2c91d6e4-3bb0-4b61-9b5c-f0b79a1d74e8"), "Ticket", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Ingresso com acesso a área VIP.", true, "Ingresso VIP", 250.00m },
                    { new Guid("8f7c0a59-7f5b-4dc2-a8fb-6b4a2e8e1c31"), "Ticket", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Ingresso comum para acesso ao evento.", true, "Ingresso Pista", 120.00m },
                    { new Guid("b57e2f13-91f6-47c9-8f8e-2a0f3d6cbb92"), "Merchandise", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Camiseta oficial do evento.", true, "Camiseta Oficial", 80.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
