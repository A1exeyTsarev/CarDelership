using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDelership.Migrations
{
    /// <inheritdoc />
    public partial class CarDelership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyTypes",
                columns: table => new
                {
                    BodyType_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyTypes", x => x.BodyType_Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Color_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Color_Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Country_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Country_Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryMethods",
                columns: table => new
                {
                    DeliveryMethods_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethods", x => x.DeliveryMethods_Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineTypes",
                columns: table => new
                {
                    EngineType_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineTypes", x => x.EngineType_Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    OrderStatus_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.OrderStatus_Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethods_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethods_Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Role_Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusSupply",
                columns: table => new
                {
                    StatusSupply_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusSupply", x => x.StatusSupply_Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Supplier_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Supplier_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Tag_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Tag_Id);
                });

            migrationBuilder.CreateTable(
                name: "Transmissions",
                columns: table => new
                {
                    Transmission_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transmissions", x => x.Transmission_Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Manufacturer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country_Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Manufacturer_Id);
                    table.ForeignKey(
                        name: "FK_Manufacturers_Countries_Country_Id",
                        column: x => x.Country_Id,
                        principalTable: "Countries",
                        principalColumn: "Country_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PassportData = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Roles",
                        principalColumn: "Role_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Model_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturer_Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BodyType_Id = table.Column<int>(type: "int", nullable: false),
                    EngineType_Id = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Transmission_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Model_Id);
                    table.ForeignKey(
                        name: "FK_Models_BodyTypes_BodyType_Id",
                        column: x => x.BodyType_Id,
                        principalTable: "BodyTypes",
                        principalColumn: "BodyType_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_EngineTypes_EngineType_Id",
                        column: x => x.EngineType_Id,
                        principalTable: "EngineTypes",
                        principalColumn: "EngineType_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Manufacturers_Manufacturer_Id",
                        column: x => x.Manufacturer_Id,
                        principalTable: "Manufacturers",
                        principalColumn: "Manufacturer_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Transmissions_Transmission_Id",
                        column: x => x.Transmission_Id,
                        principalTable: "Transmissions",
                        principalColumn: "Transmission_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderStatus_Id = table.Column<int>(type: "int", nullable: false),
                    OrderMethod_Id = table.Column<int>(type: "int", nullable: false),
                    DeliveryMethod_Id = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK_Orders_DeliveryMethods_DeliveryMethod_Id",
                        column: x => x.DeliveryMethod_Id,
                        principalTable: "DeliveryMethods",
                        principalColumn: "DeliveryMethods_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatus_Id",
                        column: x => x.OrderStatus_Id,
                        principalTable: "OrderStatuses",
                        principalColumn: "OrderStatus_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Car_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    model_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvilabilityStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color_Id = table.Column<int>(type: "int", nullable: false),
                    Mileage = table.Column<int>(type: "int", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Car_Id);
                    table.ForeignKey(
                        name: "FK_Cars_Models_model_Id",
                        column: x => x.model_Id,
                        principalTable: "Models",
                        principalColumn: "Model_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarComments",
                columns: table => new
                {
                    CarComment_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    Author_Id = table.Column<int>(type: "int", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarComments", x => x.CarComment_Id);
                    table.ForeignKey(
                        name: "FK_CarComments_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarComments_Users_Author_Id",
                        column: x => x.Author_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarTags",
                columns: table => new
                {
                    CarTag_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag_Id = table.Column<int>(type: "int", nullable: false),
                    Car_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTags", x => x.CarTag_Id);
                    table.ForeignKey(
                        name: "FK_CarTags_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarTags_Tags_Tag_Id",
                        column: x => x.Tag_Id,
                        principalTable: "Tags",
                        principalColumn: "Tag_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItem_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItem_Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Orders",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    ShoppingCart_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.ShoppingCart_Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supplies",
                columns: table => new
                {
                    Supply_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_Id = table.Column<int>(type: "int", nullable: false),
                    Car_Id = table.Column<int>(type: "int", nullable: false),
                    Status_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplies", x => x.Supply_Id);
                    table.ForeignKey(
                        name: "FK_Supplies_Cars_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Cars",
                        principalColumn: "Car_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supplies_StatusSupply_Status_Id",
                        column: x => x.Status_Id,
                        principalTable: "StatusSupply",
                        principalColumn: "StatusSupply_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supplies_Suppliers_Supplier_Id",
                        column: x => x.Supplier_Id,
                        principalTable: "Suppliers",
                        principalColumn: "Supplier_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarComments_Author_Id",
                table: "CarComments",
                column: "Author_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CarComments_Car_Id",
                table: "CarComments",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_model_Id",
                table: "Cars",
                column: "model_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CarTags_Car_Id",
                table: "CarTags",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CarTags_Tag_Id",
                table: "CarTags",
                column: "Tag_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Country_Id",
                table: "Manufacturers",
                column: "Country_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Models_BodyType_Id",
                table: "Models",
                column: "BodyType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Models_EngineType_Id",
                table: "Models",
                column: "EngineType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Models_Manufacturer_Id",
                table: "Models",
                column: "Manufacturer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Models_Transmission_Id",
                table: "Models",
                column: "Transmission_Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Car_Id",
                table: "OrderItems",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Order_Id",
                table: "OrderItems",
                column: "Order_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryMethod_Id",
                table: "Orders",
                column: "DeliveryMethod_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus_Id",
                table: "Orders",
                column: "OrderStatus_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_User_Id",
                table: "Orders",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_Car_Id",
                table: "ShoppingCarts",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_User_Id",
                table: "ShoppingCarts",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_Car_Id",
                table: "Supplies",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_Status_Id",
                table: "Supplies",
                column: "Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_Supplier_Id",
                table: "Supplies",
                column: "Supplier_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_Id",
                table: "Users",
                column: "Role_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarComments");

            migrationBuilder.DropTable(
                name: "CarTags");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "Supplies");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "StatusSupply");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "DeliveryMethods");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "BodyTypes");

            migrationBuilder.DropTable(
                name: "EngineTypes");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Transmissions");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
