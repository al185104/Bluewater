using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewater.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chargings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chargings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contributors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PhoneNumber_CountryCode = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber_Number = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber_Extension = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRegular = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BasicPay = table.Column<decimal>(type: "TEXT", nullable: false),
                    DailyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    HDMF_Con = table.Column<decimal>(type: "TEXT", nullable: false),
                    HDMF_Er = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ShiftStartTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    ShiftBreakTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    ShiftBreakEndTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    ShiftEndTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    BreakHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    DivisionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Approved1Id = table.Column<string>(type: "TEXT", nullable: true),
                    Approved2Id = table.Column<string>(type: "TEXT", nullable: true),
                    Approved3Id = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    CivilStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    BloodType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<decimal>(type: "TEXT", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ContactInfo_Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactInfo_TelNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ContactInfo_MobileNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ContactInfo_Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ContactInfo_ProvincialAddress = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ContactInfo_MothersMaidenName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactInfo_FathersName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactInfo_EmergencyContact = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactInfo_RelationshipContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContactInfo_AddressContact = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ContactInfo_TelNoContact = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ContactInfo_MobileNoContact = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EducationInfo_PrimarySchool = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_SecondarySchool = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_TertiarySchool = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_VocationalSchool = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_PrimaryDegree = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_SecondaryDegree = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_TertiaryDegree = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EducationInfo_VocationalDegree = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EmploymentInfo_DateHired = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmploymentInfo_DateRegularized = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmploymentInfo_DateResigned = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmploymentInfo_DateTerminated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmploymentInfo_TINNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmploymentInfo_SSSNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmploymentInfo_HDMFNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmploymentInfo_PHICNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmploymentInfo_BankAccount = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EmploymentInfo_HasServiceCharge = table.Column<bool>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    PositionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PayId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LevelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChargingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChargingId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    EmployeeTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LevelId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    PayId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    PositionId1 = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Chargings_ChargingId",
                        column: x => x.ChargingId,
                        principalTable: "Chargings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Chargings_ChargingId1",
                        column: x => x.ChargingId1,
                        principalTable: "Chargings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Levels_LevelId1",
                        column: x => x.LevelId1,
                        principalTable: "Levels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Pays_PayId",
                        column: x => x.PayId,
                        principalTable: "Pays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Pays_PayId1",
                        column: x => x.PayId1,
                        principalTable: "Pays",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId1",
                        column: x => x.PositionId1,
                        principalTable: "Positions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Types_EmployeeTypeId",
                        column: x => x.EmployeeTypeId,
                        principalTable: "Types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dependents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Relationship = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeaveCode = table.Column<string>(type: "TEXT", nullable: false),
                    LeaveDescription = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultCredits = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsLeaveWithPay = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCanCarryOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveCredits_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Credential = table.Column<int>(type: "INTEGER", nullable: false),
                    SupervisedGroup = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DivisionId",
                table: "Departments",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_EmployeeId",
                table: "Dependents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ChargingId",
                table: "Employees",
                column: "ChargingId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ChargingId1",
                table: "Employees",
                column: "ChargingId1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ContactInfo_Email",
                table: "Employees",
                column: "ContactInfo_Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeTypeId",
                table: "Employees",
                column: "EmployeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FirstName_LastName",
                table: "Employees",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LevelId",
                table: "Employees",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LevelId1",
                table: "Employees",
                column: "LevelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PayId",
                table: "Employees",
                column: "PayId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PayId1",
                table: "Employees",
                column: "PayId1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId1",
                table: "Employees",
                column: "PositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TypeId",
                table: "Employees",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCredits_EmployeeId",
                table: "LeaveCredits",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_SectionId",
                table: "Positions",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_DepartmentId",
                table: "Sections",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contributors");

            migrationBuilder.DropTable(
                name: "Dependents");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "LeaveCredits");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Chargings");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Pays");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
