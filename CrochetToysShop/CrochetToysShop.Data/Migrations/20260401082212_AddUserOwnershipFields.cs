using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrochetToysShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnershipFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Toys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Toys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Toys");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Toys");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");
        }
    }
}
