using Microsoft.EntityFrameworkCore.Migrations;

namespace StoresManagement.Data.Migrations
{
    public partial class AddIdentificationColumnToPurchasetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Identification",
                table: "Purchases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identification",
                table: "Purchases");
        }
    }
}
