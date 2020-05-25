using Microsoft.EntityFrameworkCore.Migrations;

namespace StoresManagement.Data.Migrations
{
    public partial class ProductandPurchaseItemsRelationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems",
                column: "ProductId",
                unique: true);
        }
    }
}
