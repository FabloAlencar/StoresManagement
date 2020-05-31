using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StoresManagement.Data.Migrations
{
    public partial class AddingRegistrationDateColumntoPurchaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Purchases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Purchases");
        }
    }
}
