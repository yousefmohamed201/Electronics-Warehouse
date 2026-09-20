using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicsWareHouse.Migrations
{
    /// <inheritdoc />
    public partial class CreateSaleItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    SaleItemID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SaleID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    ProductID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Quantity = table.Column<int>(
                        type: "int",
                        nullable: false),

                    UnitPrice = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_SaleItems",
                        x => x.SaleItemID);

                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleID",
                        column: x => x.SaleID,
                        principalTable: "Sales",
                        principalColumn: "SaleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductID",
                table: "SaleItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleID",
                table: "SaleItems",
                column: "SaleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItems");
        }
    }
}
