using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDelership.Migrations
{
    /// <inheritdoc />
    public partial class CarDelership38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_Cars_Car_Id",
                table: "Supplies");

            migrationBuilder.DropIndex(
                name: "IX_Supplies_Car_Id",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "Car_Id",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Supplies");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Supplies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Supplies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SupplyItems",
                columns: table => new
                {
                    SupplyItem_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supply_Id = table.Column<int>(type: "int", nullable: false),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyItems", x => x.SupplyItem_Id);
                    table.ForeignKey(
                        name: "FK_SupplyItems_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyItems_Supplies_Supply_Id",
                        column: x => x.Supply_Id,
                        principalTable: "Supplies",
                        principalColumn: "Supply_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_Car_Id",
                table: "SupplyItems",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItems_Supply_Id",
                table: "SupplyItems",
                column: "Supply_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplyItems");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Supplies");

            migrationBuilder.AddColumn<int>(
                name: "Car_Id",
                table: "Supplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Supplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_Car_Id",
                table: "Supplies",
                column: "Car_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_Cars_Car_Id",
                table: "Supplies",
                column: "Car_Id",
                principalTable: "Cars",
                principalColumn: "Car_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
