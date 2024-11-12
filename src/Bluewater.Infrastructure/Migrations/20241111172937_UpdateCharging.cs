using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCharging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Chargings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chargings_DepartmentId",
                table: "Chargings",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargings_Departments_DepartmentId",
                table: "Chargings",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chargings_Departments_DepartmentId",
                table: "Chargings");

            migrationBuilder.DropIndex(
                name: "IX_Chargings_DepartmentId",
                table: "Chargings");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Chargings");
        }
    }
}
