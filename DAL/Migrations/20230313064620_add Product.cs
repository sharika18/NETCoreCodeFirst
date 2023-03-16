using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ListPrice = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    StandardCost = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    SubCategoryKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubCategoryKey1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductKey);
                    table.ForeignKey(
                        name: "FK_Product_SubCategory_SubCategoryKey1",
                        column: x => x.SubCategoryKey1,
                        principalTable: "SubCategory",
                        principalColumn: "SubCategoryKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryKey1",
                table: "Product",
                column: "SubCategoryKey1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
