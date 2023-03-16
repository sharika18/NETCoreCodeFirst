using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class dropFKProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_SubCategory_SubCategoryKey1",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_SubCategoryKey1",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SubCategoryKey",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SubCategoryKey1",
                table: "Product");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryKey",
                table: "Product",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryKey1",
                table: "Product",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryKey1",
                table: "Product",
                column: "SubCategoryKey1");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_SubCategory_SubCategoryKey1",
                table: "Product",
                column: "SubCategoryKey1",
                principalTable: "SubCategory",
                principalColumn: "SubCategoryKey",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
