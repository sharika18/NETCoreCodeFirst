using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addCategoryandSubCategoryFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryKey",
                table: "SubCategory",
                type: "uniqueidentifier",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryKey",
                table: "SubCategory",
                column: "CategoryKey");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CategoryKey",
                table: "SubCategory",
                column: "CategoryKey",
                principalTable: "Category",
                principalColumn: "CategoryKey",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CategoryKey",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryKey",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "CategoryKey",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "Categorykey",
                table: "SubCategory");
        }
    }
}
