using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class droptablesalesstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_SalesStatus_SalesStatusId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "SalesStatus");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SalesStatusId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SalesStatusId",
                table: "Sales");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalesStatusId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SalesStatus",
                columns: table => new
                {
                    SalesStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesStatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesStatus", x => x.SalesStatusId);
                });

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
    }
}
