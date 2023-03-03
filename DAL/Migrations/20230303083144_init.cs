using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fakultas",
                columns: table => new
                {
                    FakultasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamaFakultas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fakultas", x => x.FakultasId);
                });

            migrationBuilder.CreateTable(
                name: "ProgramStudi",
                columns: table => new
                {
                    ProgramStudiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamaProgramStudi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FakultasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramStudi", x => x.ProgramStudiId);
                    table.ForeignKey(
                        name: "FK_ProgramStudi_Fakultas_FakultasId",
                        column: x => x.FakultasId,
                        principalTable: "Fakultas",
                        principalColumn: "FakultasId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fakultas_NamaFakultas",
                table: "Fakultas",
                column: "NamaFakultas",
                unique: true,
                filter: "[NamaFakultas] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramStudi_FakultasId",
                table: "ProgramStudi",
                column: "FakultasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramStudi");

            migrationBuilder.DropTable(
                name: "Fakultas");
        }
    }
}
