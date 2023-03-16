using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class deleteColumnAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Sales_SalesId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Sales_SalesId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Territories_Sales_SalesId",
                table: "Territories");

            migrationBuilder.DropIndex(
                name: "IX_Territories_SalesId",
                table: "Territories");

            migrationBuilder.DropIndex(
                name: "IX_Product_SalesId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Customer_SalesId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "SalesId",
                table: "Territories");

            migrationBuilder.DropColumn(
                name: "SalesId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SalesId",
                table: "Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalesId",
                table: "Territories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesId",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Territories_SalesId",
                table: "Territories",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SalesId",
                table: "Product",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_SalesId",
                table: "Customer",
                column: "SalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Sales_SalesId",
                table: "Customer",
                column: "SalesId",
                principalTable: "Sales",
                principalColumn: "SalesId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Sales_SalesId",
                table: "Product",
                column: "SalesId",
                principalTable: "Sales",
                principalColumn: "SalesId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Territories_Sales_SalesId",
                table: "Territories",
                column: "SalesId",
                principalTable: "Sales",
                principalColumn: "SalesId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
