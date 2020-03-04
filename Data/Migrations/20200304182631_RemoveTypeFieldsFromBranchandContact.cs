using Microsoft.EntityFrameworkCore.Migrations;

namespace StoresManagement.Data.Migrations
{
    public partial class RemoveTypeFieldsFromBranchandContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OwnerAddressType",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Branches");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressType",
                table: "Contacts",
                type: "nvarchar(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerAddressType",
                table: "Contacts",
                type: "nvarchar(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Branches",
                type: "nvarchar(1)",
                nullable: true);
        }
    }
}
