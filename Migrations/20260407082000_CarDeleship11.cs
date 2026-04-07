using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDelership.Migrations
{
    /// <inheritdoc />
    public partial class CarDeleship11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_StatusSupply_Status_Id",
                table: "Supplies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusSupply",
                table: "StatusSupply");

            migrationBuilder.RenameTable(
                name: "StatusSupply",
                newName: "StatusSupplies");

            migrationBuilder.RenameColumn(
                name: "AvilabilityStatus",
                table: "Cars",
                newName: "AvailabilityStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusSupplies",
                table: "StatusSupplies",
                column: "StatusSupply_Id");

            migrationBuilder.CreateTable(
                name: "CarImages",
                columns: table => new
                {
                    Image_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarImages", x => x.Image_Id);
                    table.ForeignKey(
                        name: "FK_CarImages_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Color_Id",
                table: "Cars",
                column: "Color_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_Car_Id",
                table: "CarImages",
                column: "Car_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Colors_Color_Id",
                table: "Cars",
                column: "Color_Id",
                principalTable: "Colors",
                principalColumn: "Color_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_StatusSupplies_Status_Id",
                table: "Supplies",
                column: "Status_Id",
                principalTable: "StatusSupplies",
                principalColumn: "StatusSupply_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Colors_Color_Id",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_StatusSupplies_Status_Id",
                table: "Supplies");

            migrationBuilder.DropTable(
                name: "CarImages");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Color_Id",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusSupplies",
                table: "StatusSupplies");

            migrationBuilder.RenameTable(
                name: "StatusSupplies",
                newName: "StatusSupply");

            migrationBuilder.RenameColumn(
                name: "AvailabilityStatus",
                table: "Cars",
                newName: "AvilabilityStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusSupply",
                table: "StatusSupply",
                column: "StatusSupply_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_StatusSupply_Status_Id",
                table: "Supplies",
                column: "Status_Id",
                principalTable: "StatusSupply",
                principalColumn: "StatusSupply_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
