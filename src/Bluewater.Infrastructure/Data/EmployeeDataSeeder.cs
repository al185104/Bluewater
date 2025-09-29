using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DependentAggregate;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.LeaveCreditAggregate;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.PayAggregate;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.UserAggregate;
using Bluewater.Core.UserAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data;

public static class EmployeeDataSeeder
{
  private const string PrimaryUsername = "jdoe";
  private const string SecondaryUsername = "mlopez";
  private const string DivisionName = "Hotel Operations";
  private const string DepartmentName = "Front Office";
  private const string SectionName = "Reception";
  private const string PositionName = "Front Desk Officer";
  private const string EmployeeTypeName = "Regular";
  private const string LevelName = "Level 1";
  private const string ChargingName = "Front Office Charging";
  private const decimal DefaultBasicPay = 28000m;
  private const decimal DefaultDailyRate = 1076.92m;
  private const decimal DefaultHourlyRate = 134.62m;

  public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(context);

    if (await context.Employees.AnyAsync(cancellationToken))
    {
      return;
    }

    var references = await EnsureReferenceDataAsync(context, cancellationToken);

    var employees = CreateEmployees(references);
    context.Employees.AddRange(employees);

    var dependents = CreateDependents(employees.First());
    context.Dependents.AddRange(dependents);

    await context.SaveChangesAsync(cancellationToken);
  }

  private static async Task<SeedReferenceData> EnsureReferenceDataAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var users = await EnsureAppUsersAsync(context, cancellationToken);
    var division = await GetOrCreateDivisionAsync(context, cancellationToken);
    var department = await GetOrCreateDepartmentAsync(context, division, cancellationToken);
    var section = await GetOrCreateSectionAsync(context, department, cancellationToken);
    var position = await GetOrCreatePositionAsync(context, section, cancellationToken);
    var pay = await GetOrCreatePayAsync(context, cancellationToken);
    var employeeType = await GetOrCreateEmployeeTypeAsync(context, cancellationToken);
    var level = await GetOrCreateLevelAsync(context, cancellationToken);
    var charging = await GetOrCreateChargingAsync(context, department, cancellationToken);
    await EnsureLeaveCreditsAsync(context, cancellationToken);

    return new SeedReferenceData(users.PrimaryUser, users.SecondaryUser, position, pay, employeeType, level, charging);
  }

  private static async Task<(AppUser PrimaryUser, AppUser SecondaryUser)> EnsureAppUsersAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var primaryUser = await context.AppUsers.FirstOrDefaultAsync(u => u.Username == PrimaryUsername, cancellationToken);
    var secondaryUser = await context.AppUsers.FirstOrDefaultAsync(u => u.Username == SecondaryUsername, cancellationToken);

    var hasChanges = false;

    if (primaryUser is null)
    {
      primaryUser = new AppUser(PrimaryUsername, "HASHED_PASSWORD", Credential.Employee, null);
      context.AppUsers.Add(primaryUser);
      hasChanges = true;
    }

    if (secondaryUser is null)
    {
      secondaryUser = new AppUser(SecondaryUsername, "HASHED_PASSWORD", Credential.Supervisor, null, true);
      context.AppUsers.Add(secondaryUser);
      hasChanges = true;
    }

    if (hasChanges)
    {
      await context.SaveChangesAsync(cancellationToken);
    }

    return (primaryUser!, secondaryUser!);
  }

  private static async Task<Division> GetOrCreateDivisionAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var division = await context.Divisions.FirstOrDefaultAsync(d => d.Name == DivisionName, cancellationToken);

    if (division is null)
    {
      division = new Division(DivisionName, "Oversees resort operations");
      context.Divisions.Add(division);
      await context.SaveChangesAsync(cancellationToken);
    }

    return division;
  }

  private static async Task<Department> GetOrCreateDepartmentAsync(AppDbContext context, Division division, CancellationToken cancellationToken)
  {
    var department = await context.Departments.FirstOrDefaultAsync(d => d.Name == DepartmentName, cancellationToken);

    if (department is null)
    {
      department = new Department(DepartmentName, "Responsible for guest engagement", division.Id);
      context.Departments.Add(department);
      await context.SaveChangesAsync(cancellationToken);
    }

    return department;
  }

  private static async Task<Section> GetOrCreateSectionAsync(AppDbContext context, Department department, CancellationToken cancellationToken)
  {
    var section = await context.Sections.FirstOrDefaultAsync(s => s.Name == SectionName, cancellationToken);

    if (section is null)
    {
      section = new Section(SectionName, "Front desk reception", null, null, null, department.Id);
      context.Sections.Add(section);
      await context.SaveChangesAsync(cancellationToken);
    }

    return section;
  }

  private static async Task<Position> GetOrCreatePositionAsync(AppDbContext context, Section section, CancellationToken cancellationToken)
  {
    var position = await context.Positions.FirstOrDefaultAsync(p => p.Name == PositionName, cancellationToken);

    if (position is null)
    {
      position = new Position(PositionName, "Greets guests and manages check-in/out", section.Id);
      context.Positions.Add(position);
      await context.SaveChangesAsync(cancellationToken);
    }

    return position;
  }

  private static async Task<Pay> GetOrCreatePayAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var pay = await context.Pays.FirstOrDefaultAsync(p => p.BasicPay == DefaultBasicPay && p.DailyRate == DefaultDailyRate && p.HourlyRate == DefaultHourlyRate, cancellationToken);

    if (pay is null)
    {
      pay = new Pay(DefaultBasicPay, DefaultDailyRate, DefaultHourlyRate);
      context.Pays.Add(pay);
      await context.SaveChangesAsync(cancellationToken);
    }

    return pay;
  }

  private static async Task<EmployeeType> GetOrCreateEmployeeTypeAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var type = await context.Types.FirstOrDefaultAsync(t => t.Name == EmployeeTypeName, cancellationToken);

    if (type is null)
    {
      type = new EmployeeType(EmployeeTypeName, "REG", true);
      context.Types.Add(type);
      await context.SaveChangesAsync(cancellationToken);
    }

    return type;
  }

  private static async Task<Level> GetOrCreateLevelAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var level = await context.Levels.FirstOrDefaultAsync(l => l.Name == LevelName, cancellationToken);

    if (level is null)
    {
      level = new Level(LevelName, "1", true);
      context.Levels.Add(level);
      await context.SaveChangesAsync(cancellationToken);
    }

    return level;
  }

  private static async Task<Charging> GetOrCreateChargingAsync(AppDbContext context, Department department, CancellationToken cancellationToken)
  {
    var charging = await context.Chargings.FirstOrDefaultAsync(c => c.Name == ChargingName, cancellationToken);

    if (charging is null)
    {
      charging = new Charging(ChargingName, "Default charging for front office", department.Id);
      context.Chargings.Add(charging);
      await context.SaveChangesAsync(cancellationToken);
    }

    return charging;
  }

  private static async Task EnsureLeaveCreditsAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var created = false;

    if (!await context.LeaveCredits.AnyAsync(lc => lc.LeaveCode == "VL", cancellationToken))
    {
      context.LeaveCredits.Add(new LeaveCredit("VL", "Vacation Leave", 10, true, true, 1));
      created = true;
    }

    if (!await context.LeaveCredits.AnyAsync(lc => lc.LeaveCode == "SL", cancellationToken))
    {
      context.LeaveCredits.Add(new LeaveCredit("SL", "Sick Leave", 10, true, false, 2));
      created = true;
    }

    if (created)
    {
      await context.SaveChangesAsync(cancellationToken);
    }
  }

  private static List<Employee> CreateEmployees(SeedReferenceData references)
  {
    var primaryEmployee = new Employee(
      firstName: "John",
      lastName: "Doe",
      middleName: "A",
      dateOfBirth: new DateTime(1985, 3, 14),
      gender: Gender.Male,
      civilStatus: CivilStatus.Married,
      bloodType: BloodType.OPositive,
      status: Status.Active,
      height: 175m,
      weight: 72m,
      imageUrl: null,
      remarks: "Front desk lead",
      mealCredits: 5,
      tenant: Tenant.Maribago);

    primaryEmployee.SetContactInfo(new ContactInfo(
      email: "john.doe@example.com",
      telNumber: "032-123-4567",
      mobileNumber: "0917-123-4567",
      address: "123 Resort Drive, Lapu-Lapu",
      provincialAddress: "456 Province Road, Cebu",
      mothersMaidenName: "Mary Smith",
      fathersName: "Michael Doe",
      emergencyContact: "Jane Doe",
      relationshipContact: "Spouse",
      addressContact: "123 Resort Drive, Lapu-Lapu",
      telNoContact: "032-765-4321",
      mobileNoContact: "0917-987-6543"));

    primaryEmployee.SetEducationInfo(new EducationInfo(
      educationalAttainment: EducationalAttainment.Bachelors,
      courseGraduated: "Hospitality Management",
      universityGraduated: "Cebu State University"));

    primaryEmployee.SetEmploymentInfo(new EmploymentInfo(
      dateHired: new DateTime(2015, 6, 1),
      dateRegularized: new DateTime(2015, 12, 1),
      dateResigned: null,
      dateTerminated: null,
      tinNo: "123-456-789",
      sssNo: "34-5678901-2",
      hdmfNo: "HM-123456789",
      phicNo: "PH-987654321",
      bankAccount: "0123456789",
      hasServiceCharge: true));

    primaryEmployee.SetExternalKeys(
      references.PrimaryUser.Id,
      references.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      references.Charging.Id);

    var secondaryEmployee = new Employee(
      firstName: "Maria",
      lastName: "Lopez",
      middleName: "B",
      dateOfBirth: new DateTime(1990, 7, 22),
      gender: Gender.Female,
      civilStatus: CivilStatus.Single,
      bloodType: BloodType.APositive,
      status: Status.Active,
      height: 165m,
      weight: 60m,
      imageUrl: null,
      remarks: "Handles concierge requests",
      mealCredits: 3,
      tenant: Tenant.Panglao);

    secondaryEmployee.SetContactInfo(new ContactInfo(
      email: "maria.lopez@example.com",
      telNumber: "038-222-3344",
      mobileNumber: "0920-555-7788",
      address: "78 Beachfront Ave, Panglao",
      provincialAddress: "78 Beachfront Ave, Bohol",
      mothersMaidenName: "Angela Cruz",
      fathersName: "Roberto Lopez",
      emergencyContact: "Ana Lopez",
      relationshipContact: "Sister",
      addressContact: "78 Beachfront Ave, Bohol",
      telNoContact: "038-555-6677",
      mobileNoContact: "0920-999-8877"));

    secondaryEmployee.SetEducationInfo(new EducationInfo(
      educationalAttainment: EducationalAttainment.College,
      courseGraduated: "Tourism",
      universityGraduated: "Bohol Tourism College"));

    secondaryEmployee.SetEmploymentInfo(new EmploymentInfo(
      dateHired: new DateTime(2019, 3, 15),
      dateRegularized: new DateTime(2019, 9, 15),
      dateResigned: null,
      dateTerminated: null,
      tinNo: "987-654-321",
      sssNo: "45-6789012-3",
      hdmfNo: "HM-987654321",
      phicNo: "PH-123456789",
      bankAccount: "9876543210",
      hasServiceCharge: false));

    secondaryEmployee.SetExternalKeys(
      references.SecondaryUser.Id,
      references.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      references.Charging.Id);

    return new List<Employee> { primaryEmployee, secondaryEmployee };
  }

  private static IEnumerable<Dependent> CreateDependents(Employee employee)
  {
    return new List<Dependent>
    {
      new Dependent("Jane", "Doe", "Spouse", new DateTime(1986, 5, 10))
      {
        Employee = employee
      },
      new Dependent("Jimmy", "Doe", "Child", new DateTime(2015, 4, 3))
      {
        Employee = employee
      }
    };
  }

  private record SeedReferenceData(AppUser PrimaryUser, AppUser SecondaryUser, Position Position, Pay Pay, EmployeeType EmployeeType, Level Level, Charging Charging);
}
