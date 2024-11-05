using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatepayrolls02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Undertimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "InclusiveTime",
                table: "Undertimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Undertimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "Cola",
                table: "Pays",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Overtimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Overtimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Overtimes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedHours",
                table: "Overtimes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "OtherEarnings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EarningType",
                table: "OtherEarnings",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "OtherEarnings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "FailureInOuts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Reason",
                table: "FailureInOuts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "FailureInOuts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainingBalance",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "NoOfMonths",
                table: "Deductions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyAmortization",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Deductions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "NightShiftHours",
                table: "Attendance",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverbreakHrs",
                table: "Attendance",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    GrossPayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    BasicPayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SSSAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SSSERAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PagibigAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PagibigERAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PhilhealthAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PhilhealthERAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RestDayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RestDayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    RegularHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RegularHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    SpecialHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SpecialHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffOvertimeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffOvertimeHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffRegularHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffRegularHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffSpecialHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NightDiffSpecialHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeRestDayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeRestDayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeRegularHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeRegularHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeSpecialHolidayAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeSpecialHolidayHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnionDues = table.Column<decimal>(type: "TEXT", nullable: false),
                    Absences = table.Column<int>(type: "INTEGER", nullable: false),
                    AbsencesAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Leaves = table.Column<decimal>(type: "TEXT", nullable: false),
                    LeavesAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Lates = table.Column<decimal>(type: "TEXT", nullable: false),
                    LatesAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Undertime = table.Column<decimal>(type: "TEXT", nullable: false),
                    UndertimeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Overbreak = table.Column<decimal>(type: "TEXT", nullable: false),
                    OverbreakAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SvcCharge = table.Column<decimal>(type: "TEXT", nullable: false),
                    CostOfLivingAllowanceAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthlyAllowanceAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SalaryUnderpaymentAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundAbsencesAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundUndertimeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundOvertimeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    LaborHoursIncome = table.Column<decimal>(type: "TEXT", nullable: false),
                    LaborHrs = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxDeductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalConstantDeductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalLoanDeductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payrolls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Cola",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "NightShiftHours",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "OverbreakHrs",
                table: "Attendance");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Undertimes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InclusiveTime",
                table: "Undertimes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Undertimes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Overtimes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Overtimes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Overtimes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedHours",
                table: "Overtimes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "OtherEarnings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EarningType",
                table: "OtherEarnings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "OtherEarnings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "FailureInOuts",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Reason",
                table: "FailureInOuts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "FailureInOuts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainingBalance",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NoOfMonths",
                table: "Deductions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyAmortization",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Deductions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
