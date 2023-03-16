using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addFKSalesStatusId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalesStatusId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SalesStatusId",
                table: "Sales",
                column: "SalesStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_SalesStatus_SalesStatusId",
                table: "Sales",
                column: "SalesStatusId",
                principalTable: "SalesStatus",
                principalColumn: "SalesStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_SalesStatus_SalesStatusId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SalesStatusId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SalesStatusId",
                table: "Sales");
        }
    }
}
