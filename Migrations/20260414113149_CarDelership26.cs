using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDelership.Migrations
{
    /// <inheritdoc />
    public partial class CarDelership26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "CarComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "CarComments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "CarComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "CarComments");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "CarComments");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "CarComments");
        }
    }
}
