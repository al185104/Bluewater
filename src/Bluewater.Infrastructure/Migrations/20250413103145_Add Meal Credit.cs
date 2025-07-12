using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealCredit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealCredits",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tenant",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_LeaveCreditId",
                table: "Leaves",
                column: "LeaveCreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_LeaveCredits_LeaveCreditId",
                table: "Leaves",
                column: "LeaveCreditId",
                principalTable: "LeaveCredits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_LeaveCredits_LeaveCreditId",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_LeaveCreditId",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "MealCredits",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Tenant",
                table: "Employees");
        }
    }
}
