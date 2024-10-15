using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateemployeetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationInfo_PrimaryDegree",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationInfo_PrimarySchool",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationInfo_SecondaryDegree",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationInfo_SecondarySchool",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationInfo_TertiaryDegree",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationInfo_TertiarySchool",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "EducationInfo_VocationalSchool",
                table: "Employees",
                newName: "EducationInfo_UniversityGraduated");

            migrationBuilder.RenameColumn(
                name: "EducationInfo_VocationalDegree",
                table: "Employees",
                newName: "EducationInfo_CourseGraduated");

            migrationBuilder.AddColumn<int>(
                name: "EducationInfo_EducationalAttainment",
                table: "Employees",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationInfo_EducationalAttainment",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "EducationInfo_UniversityGraduated",
                table: "Employees",
                newName: "EducationInfo_VocationalSchool");

            migrationBuilder.RenameColumn(
                name: "EducationInfo_CourseGraduated",
                table: "Employees",
                newName: "EducationInfo_VocationalDegree");

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_PrimaryDegree",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_PrimarySchool",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_SecondaryDegree",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_SecondarySchool",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_TertiaryDegree",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationInfo_TertiarySchool",
                table: "Employees",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }
    }
}
