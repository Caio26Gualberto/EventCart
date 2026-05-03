using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace inventory_service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "CreatedAt", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("8c7a2bb8-8e0c-4a01-9a91-e5bcf55cf011"), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("8f7c0a59-7f5b-4dc2-a8fb-6b4a2e8e1c31"), 100 },
                    { new Guid("d4f6d991-5ef7-4dd4-a2f2-772994d55a5d"), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b57e2f13-91f6-47c9-8f8e-2a0f3d6cbb92"), 30 },
                    { new Guid("efcbf096-7353-4803-84f4-a497c605d581"), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2c91d6e4-3bb0-4b61-9b5c-f0b79a1d74e8"), 50 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProductId",
                table: "InventoryItems",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");
        }
    }
}
