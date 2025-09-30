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
  private const string TertiaryUsername = "kchan";
  private const string QuaternaryUsername = "areyes";
  private static readonly IReadOnlyList<DivisionSeed> OrganizationSeeds = new List<DivisionSeed>
  {
    new("AG", new List<DepartmentSeed>
    {
      new("Accounting", new List<ChargingSeed>
      {
        new("FD", new List<SectionSeed>
        {
          new("FDCostC", new List<string> { "Accounting Associate" }),
          new("FDGA", new List<string> { "Accounting Associate", "Junior Accountant", "Receiving Clerk (Warehouse)" }),
          new("FDTC", new List<string> { "Accounting Associate" }),
          new("FTAC", new List<string> { "General Cashier" }),
          new("ADMIN", new List<string> { "Liaison Officer" })
        })
      }),
      new("IT", new List<ChargingSeed>
      {
        new("FD", new List<SectionSeed>
        {
          new("ITD", new List<string> { "IT Specialist", "IT Supervisor", "Assistant IT Manager", "IT Manager" })
        })
      }),
      new("Purchasing", new List<ChargingSeed>
      {
        new("MMO", new List<SectionSeed>
        {
          new("MMO", new List<string> { "Purchaser", "Purchasing Associate", "Purchasing Supervisor", "Stock Clerk", "Driver/Purchaser" })
        })
      }),
      new("SSD", new List<ChargingSeed>
      {
        new("SSD", new List<SectionSeed>
        {
          new("SSD", new List<string> { "Safety & Security Supervisor", "Safety & Security Manager", "Security Team Leader" })
        }),
        new("SSD_A", new List<SectionSeed>
        {
          new("SSD", new List<string> { "House Detective" })
        })
      })
    }),
    new("SM", new List<DepartmentSeed>
    {
      new("Banquet Sales", new List<ChargingSeed>
      {
        new("BSC", new List<SectionSeed>
        {
          new("BQT", new List<string> { "Banquet Sales Coordinator", "Sales Account Manager", "Sales Account Executive" })
        })
      }),
      new("MarComm", new List<ChargingSeed>
      {
        new("SM", new List<SectionSeed>
        {
          new("SM", new List<string> { "Graphic Designer", "Senior Graphic Designer", "Copywriter", "MarComm Associate" })
        })
      }),
      new("Reservations", new List<ChargingSeed>
      {
        new("ROC", new List<SectionSeed>
        {
          new("ROC", new List<string> { "Reservations Officer (Cebu)", "Reservations Associate" })
        }),
        new("SM", new List<SectionSeed>
        {
          new("SM", new List<string> { "Admin Staff" })
        }),
        new("SMM", new List<SectionSeed>
        {
          new("ROM", new List<string> { "Reservations Officer (Manila)", "Reservations Specialist (Manila)", "Reservations Team Leader", "Assistant Reservations Manager", "Reservations Manager", "Rooms Revenue Manager" })
        })
      })
    }),
    new("CO", new List<DepartmentSeed>
    {
      new("Corporate", new List<ChargingSeed>
      {
        new("COD", new List<SectionSeed>
        {
          new("90001", new List<string> { "Liaison Officer" })
        })
      })
    }),
    new("FBDIV", new List<DepartmentSeed>
    {
      new("F&B Production", new List<ChargingSeed>
      {
        new("FBAll", new List<SectionSeed>
        {
          new("AP", new List<string> { "Chef de Partie", "Commis II", "Pantry II" }),
          new("AS", new List<string> { "On-call Steward" })
        }),
        new("FBAll_A", new List<SectionSeed>
        {
          new("AP", new List<string> { "Commis III" })
        }),
        new("FBASC", new List<SectionSeed>
        {
          new("KPB", new List<string> { "Baker II", "Baker III", "Pastry II" }),
          new("KP", new List<string> { "Commis I", "Commis II" }),
          new("KS", new List<string> { "Stewarding Supervisor" })
        }),
        new("FBASC_A", new List<SectionSeed>
        {
          new("KP", new List<string> { "Commis III" })
        }),
        new("FBBan", new List<SectionSeed>
        {
          new("BCP", new List<string> { "Commis II" })
        }),
        new("FBCo", new List<SectionSeed>
        {
          new("COP", new List<string> { "Chef de Partie", "Commis II" })
        }),
        new("FBMJ", new List<SectionSeed>
        {
          new("MJP", new List<string> { "Commis II" })
        }),
        new("FBMJ_A", new List<SectionSeed>
        {
          new("MJP", new List<string> { "Commis III" })
        })
      }),
      new("F&B Service", new List<ChargingSeed>
      {
        new("FBAll", new List<SectionSeed>
        {
          new("AS", new List<string> { "Bar Captain", "Bartender", "F&B Service Associate", "On-call Waiter" })
        }),
        new("FBASC", new List<SectionSeed>
        {
          new("ADMO", new List<string> { "F&B Supervisor" })
        }),
        new("FBBan", new List<SectionSeed>
        {
          new("BCS", new List<string> { "Banquet Captain", "Banquet Operation Supervisor", "F&B Service Associate" })
        }),
        new("FBCo", new List<SectionSeed>
        {
          new("COS", new List<string> { "Bartender", "Outlet Cashier", "F&B Service Associate" })
        }),
        new("FBRecA", new List<SectionSeed>
        {
          new("Recbar", new List<string> { "F&B Service Associate" })
        })
      })
    }),
    new("RMDIV", new List<DepartmentSeed>
    {
      new("Front Office", new List<ChargingSeed>
      {
        new("FO", new List<SectionSeed>
        {
          new("BS", new List<string> { "Airport Representative", "Bellman" }),
          new("GT", new List<string> { "Bellman" }),
          new("FDC", new List<string> { "Front Office Associate" }),
          new("TO", new List<string> { "Front Office Associate" })
        })
      }),
      new("Grounds & Garden", new List<ChargingSeed>
      {
        new("GG", new List<SectionSeed>
        {
          new("GRN", new List<string> { "Garden Staff" }),
          new("GRD", new List<string> { "Grounds & Garden Associate", "Ground Associate", "Grounds & Garden Specialist", "Grounds & Garden Supervisor" })
        })
      }),
      new("Housekeeping", new List<ChargingSeed>
      {
        new("HSKPNG", new List<SectionSeed>
        {
          new("HADM", new List<string> { "Housekeeping Coordinator", "Housekeeping Team Leader", "Housekeeping Supervisor" }),
          new("GR", new List<string> { "Linen Attendant", "Room Attendant", "Senior Room Attendant" }),
          new("PA", new List<string> { "Public Area Attendant" })
        })
      })
    }),
    new("HRD", new List<DepartmentSeed>
    {
      new("Housekeeping", new List<ChargingSeed>
      {
        new("HRD", new List<SectionSeed>
        {
          new("HRS", new List<string> { "Public Area Attendant" })
        })
      }),
      new("HR", new List<ChargingSeed>
      {
        new("HRD", new List<SectionSeed>
        {
          new("HRS", new List<string> { "Company Nurse", "Corporate Recruiter", "Corporate Training Manager", "Corporate Assistant OD Manager", "Assistant HR Manager", "HR Manager", "OD Manager", "HR Associate", "HR Associate - CompBen", "CompBen Specialist", "HR Officer", "Training & Development Officer", "Assistant Training & Development Manager" })
        })
      })
    }),
    new("POMEC", new List<DepartmentSeed>
    {
      new("POMEC", new List<ChargingSeed>
      {
        new("POMEC", new List<SectionSeed>
        {
          new("BM", new List<string> { "Carpenter", "Function Team/Helper", "Mason", "Mechanic", "Painter", "Plumber", "POMEC Special Project", "RAC Technician", "Welder" }),
          new("ES", new List<string> { "Electrical Technician", "RAC Technician" }),
          new("PADM", new List<string> { "Plan Systems Engineer/Contracted Services", "POMEC Coordinator" })
        }),
        new("SP", new List<SectionSeed>
        {
          new("SPS", new List<string> { "Electrical Engineer", "Quantity Surveyor", "Shift Engineer" })
        }),
        new("FBASC", new List<SectionSeed>
        {
          new("ADMK", new List<string> { "Electronic Technician" })
        })
      })
    }),
    new("OOD", new List<DepartmentSeed>
    {
      new("SSD", new List<ChargingSeed>
      {
        new("SR", new List<SectionSeed>
        {
          new("SR", new List<string> { "Lifeguard/Pool Attendant" })
        })
      })
    })
  };

  private static readonly IReadOnlyDictionary<string, EmployeeAssignment> EmployeeAssignments = new Dictionary<string, EmployeeAssignment>(StringComparer.OrdinalIgnoreCase)
  {
    [PrimaryUsername] = new("AG", "Accounting", "FD", "FDGA", "Accounting Associate"),
    [SecondaryUsername] = new("SM", "Reservations", "ROC", "ROC", "Reservations Associate"),
    [TertiaryUsername] = new("FBDIV", "F&B Production", "FBAll", "AP", "Commis II"),
    [QuaternaryUsername] = new("HRD", "HR", "HRD", "HRS", "HR Manager")
  };

  private static string BuildKey(params string[] values) => string.Join("::", values);

  private const string EmployeeTypeName = "Regular";
  private const string LevelName = "Associate";
  private const decimal DefaultBasicPay = 28000m;
  private const decimal DefaultDailyRate = 1076.92m;
  private const decimal DefaultHourlyRate = 134.62m;

  private static readonly IReadOnlyList<EmployeeTypeSeedInfo> EmployeeTypeSeeds = new List<EmployeeTypeSeedInfo>
  {
    new("Regular", "REG"),
    new("Probationary", "PROBATIONARY"),
    new("Confi", "CONFI"),
    new("On leave", "ON_LEAVE"),
    new("On call", "ON_CALL"),
    new("OJT", "OJT"),
    new("Separated", "SEPARATED"),
    new("Managers", "MANAGERS"),
    new("Blue Bubble", "BLUE_BUBBLE"),
    new("AHI", "AHI"),
    new("ACGSI", "ACGSI"),
    new("Special Project", "SPECIAL_PROJECT"),
    new("Agency", "AGENCY"),
    new("Amuma", "AMUMA")
  };

  private static readonly IReadOnlyList<LevelSeedInfo> LevelSeeds = new List<LevelSeedInfo>
  {
    new("Associate", "ASSOCIATE"),
    new("Manager", "MANAGER"),
    new("Supervisor", "SUPERVISOR")
  };

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
    var organization = await EnsureOrganizationStructureAsync(context, cancellationToken);
    var pay = await GetOrCreatePayAsync(context, cancellationToken);
    var employeeType = await GetOrCreateEmployeeTypeAsync(context, cancellationToken);
    var level = await GetOrCreateLevelAsync(context, cancellationToken);
    await EnsureLeaveCreditsAsync(context, cancellationToken);

    return new SeedReferenceData(users, pay, employeeType, level, organization);
  }

  private static readonly IReadOnlyList<AppUserSeedInfo> AppUsersToSeed = new List<AppUserSeedInfo>
  {
    new(PrimaryUsername, Credential.Employee, null, false),
    new(SecondaryUsername, Credential.Supervisor, null, true),
    new(TertiaryUsername, Credential.Employee, null, false),
    new(QuaternaryUsername, Credential.Manager, null, false)
  };

  private static async Task<IReadOnlyDictionary<string, AppUser>> EnsureAppUsersAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var users = new Dictionary<string, AppUser>();
    var hasChanges = false;

    foreach (var appUser in AppUsersToSeed)
    {
      var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Username == appUser.Username, cancellationToken);

      if (user is null)
      {
        user = new AppUser(appUser.Username, "HASHED_PASSWORD", appUser.Credential, appUser.SupervisedGroup, appUser.IsGlobalSupervisor);
        context.AppUsers.Add(user);
        hasChanges = true;
      }

      users[appUser.Username] = user;
    }

    if (hasChanges)
    {
      await context.SaveChangesAsync(cancellationToken);
    }

    return users;
  }

  private static async Task<OrganizationSeedResult> EnsureOrganizationStructureAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var divisions = new Dictionary<string, Division>(StringComparer.OrdinalIgnoreCase);
    var departments = new Dictionary<string, Department>(StringComparer.OrdinalIgnoreCase);
    var chargings = new Dictionary<string, Charging>(StringComparer.OrdinalIgnoreCase);
    var sections = new Dictionary<string, Section>(StringComparer.OrdinalIgnoreCase);
    var positions = new Dictionary<string, Position>(StringComparer.OrdinalIgnoreCase);

    foreach (var divisionSeed in OrganizationSeeds)
    {
      var division = await context.Divisions.FirstOrDefaultAsync(d => d.Name == divisionSeed.Division, cancellationToken);

      if (division is null)
      {
        division = new Division(divisionSeed.Division, null);
        context.Divisions.Add(division);
        await context.SaveChangesAsync(cancellationToken);
      }

      divisions[BuildKey(divisionSeed.Division)] = division;

      foreach (var departmentSeed in divisionSeed.Departments)
      {
        var department = await context.Departments.FirstOrDefaultAsync(d => d.Name == departmentSeed.Department && d.DivisionId == division.Id, cancellationToken);

        if (department is null)
        {
          department = new Department(departmentSeed.Department, null, division.Id);
          context.Departments.Add(department);
          await context.SaveChangesAsync(cancellationToken);
        }

        departments[BuildKey(divisionSeed.Division, departmentSeed.Department)] = department;

        foreach (var chargingSeed in departmentSeed.Chargings)
        {
          var charging = await context.Chargings.FirstOrDefaultAsync(c => c.Name == chargingSeed.Charging && c.DepartmentId == department.Id, cancellationToken);

          if (charging is null)
          {
            charging = new Charging(chargingSeed.Charging, null, department.Id);
            context.Chargings.Add(charging);
            await context.SaveChangesAsync(cancellationToken);
          }

          chargings[BuildKey(divisionSeed.Division, departmentSeed.Department, chargingSeed.Charging)] = charging;

          foreach (var sectionSeed in chargingSeed.Sections)
          {
            var section = await context.Sections.FirstOrDefaultAsync(s => s.Name == sectionSeed.Section && s.DepartmentId == department.Id, cancellationToken);

            if (section is null)
            {
              section = new Section(sectionSeed.Section, null, null, null, null, department.Id);
              context.Sections.Add(section);
              await context.SaveChangesAsync(cancellationToken);
            }

            sections[BuildKey(divisionSeed.Division, departmentSeed.Department, sectionSeed.Section)] = section;

            foreach (var positionName in sectionSeed.Positions)
            {
              var position = await context.Positions.FirstOrDefaultAsync(p => p.Name == positionName && p.SectionId == section.Id, cancellationToken);

              if (position is null)
              {
                position = new Position(positionName, null, section.Id);
                context.Positions.Add(position);
                await context.SaveChangesAsync(cancellationToken);
              }

              positions[BuildKey(divisionSeed.Division, departmentSeed.Department, sectionSeed.Section, positionName)] = position;
            }
          }
        }
      }
    }

    return new OrganizationSeedResult(divisions, departments, chargings, sections, positions);
  }

  private static (Position Position, Charging Charging) ResolveOrganizationReferences(SeedReferenceData references, string username)
  {
    if (!EmployeeAssignments.TryGetValue(username, out var assignment))
    {
      throw new InvalidOperationException($"No organization assignment configured for user '{username}'.");
    }

    var positionKey = BuildKey(assignment.Division, assignment.Department, assignment.Section, assignment.Position);

    if (!references.Organization.Positions.TryGetValue(positionKey, out var position))
    {
      throw new InvalidOperationException($"Position '{assignment.Position}' for section '{assignment.Section}' under department '{assignment.Department}' in division '{assignment.Division}' was not found.");
    }

    var chargingKey = BuildKey(assignment.Division, assignment.Department, assignment.Charging);

    if (!references.Organization.Chargings.TryGetValue(chargingKey, out var charging))
    {
      throw new InvalidOperationException($"Charging '{assignment.Charging}' for department '{assignment.Department}' in division '{assignment.Division}' was not found.");
    }

    return (position, charging);
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
    EmployeeType? defaultType = null;
    var hasChanges = false;

    foreach (var seed in EmployeeTypeSeeds)
    {
      var type = await context.Types.FirstOrDefaultAsync(t => t.Name == seed.Name, cancellationToken);

      if (type is null)
      {
        type = new EmployeeType(seed.Name, seed.Value, true);
        context.Types.Add(type);
        hasChanges = true;
      }

      if (seed.Name.Equals(EmployeeTypeName, StringComparison.OrdinalIgnoreCase))
      {
        defaultType = type;
      }
    }

    if (hasChanges)
    {
      await context.SaveChangesAsync(cancellationToken);
    }

    return defaultType ?? throw new InvalidOperationException("Default employee type could not be created.");
  }

  private static async Task<Level> GetOrCreateLevelAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    Level? defaultLevel = null;
    var hasChanges = false;

    foreach (var seed in LevelSeeds)
    {
      var level = await context.Levels.FirstOrDefaultAsync(l => l.Name == seed.Name, cancellationToken);

      if (level is null)
      {
        level = new Level(seed.Name, seed.Value, true);
        context.Levels.Add(level);
        hasChanges = true;
      }

      if (seed.Name.Equals(LevelName, StringComparison.OrdinalIgnoreCase))
      {
        defaultLevel = level;
      }
    }

    if (hasChanges)
    {
      await context.SaveChangesAsync(cancellationToken);
    }

    return defaultLevel ?? throw new InvalidOperationException("Default level could not be created.");
  }

  private static async Task EnsureLeaveCreditsAsync(AppDbContext context, CancellationToken cancellationToken)
  {
    var defaultLeaveCredits = new (string Code, string Description, decimal DefaultCredits, bool IsWithPay, bool IsCarryOver, int SortOrder)[]
    {
      ("VL", "Vacation Leave", 10, true, true, 1),
      ("SL", "Sick Leave", 10, true, false, 2),
      ("ML", "Maternity Leave", 105, true, false, 3),
      ("PL", "Paternity Leave", 7, true, false, 4),
      ("EL", "Emergency Leave", 5, true, false, 5)
    };

    var hasChanges = false;

    foreach (var leave in defaultLeaveCredits)
    {
      if (!await context.LeaveCredits.AnyAsync(lc => lc.LeaveCode == leave.Code, cancellationToken))
      {
        context.LeaveCredits.Add(new LeaveCredit(leave.Code, leave.Description, leave.DefaultCredits, leave.IsWithPay, leave.IsCarryOver, leave.SortOrder));
        hasChanges = true;
      }
    }

    if (hasChanges)
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
      remarks: "Leads general accounting operations",
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

    var primaryReferences = ResolveOrganizationReferences(references, PrimaryUsername);

    primaryEmployee.SetExternalKeys(
      references.Users[PrimaryUsername].Id,
      primaryReferences.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      primaryReferences.Charging.Id);

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
      remarks: "Coordinates guest reservations and inquiries",
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

    var secondaryReferences = ResolveOrganizationReferences(references, SecondaryUsername);

    secondaryEmployee.SetExternalKeys(
      references.Users[SecondaryUsername].Id,
      secondaryReferences.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      secondaryReferences.Charging.Id);

    var tertiaryEmployee = new Employee(
      firstName: "Kevin",
      lastName: "Chan",
      middleName: "D",
      dateOfBirth: new DateTime(1992, 11, 2),
      gender: Gender.Male,
      civilStatus: CivilStatus.Single,
      bloodType: BloodType.BPositive,
      status: Status.Active,
      height: 170m,
      weight: 68m,
      imageUrl: null,
      remarks: "Supports hot kitchen production for F&B",
      mealCredits: 4,
      tenant: Tenant.Sumilon);

    tertiaryEmployee.SetContactInfo(new ContactInfo(
      email: "kevin.chan@example.com",
      telNumber: "032-789-4561",
      mobileNumber: "0918-222-3344",
      address: "88 Marina Way, Lapu-Lapu",
      provincialAddress: "18 Coastal Road, Cebu",
      mothersMaidenName: "Lisa Wong",
      fathersName: "Richard Chan",
      emergencyContact: "Liam Chan",
      relationshipContact: "Brother",
      addressContact: "88 Marina Way, Lapu-Lapu",
      telNoContact: "032-789-8899",
      mobileNoContact: "0918-555-6677"));

    tertiaryEmployee.SetEducationInfo(new EducationInfo(
      educationalAttainment: EducationalAttainment.Bachelors,
      courseGraduated: "Information Technology",
      universityGraduated: "University of Cebu"));

    tertiaryEmployee.SetEmploymentInfo(new EmploymentInfo(
      dateHired: new DateTime(2020, 1, 10),
      dateRegularized: new DateTime(2020, 7, 10),
      dateResigned: null,
      dateTerminated: null,
      tinNo: "112-233-445",
      sssNo: "55-7890123-4",
      hdmfNo: "HM-223344556",
      phicNo: "PH-223344556",
      bankAccount: "5566778899",
      hasServiceCharge: true));

    var tertiaryReferences = ResolveOrganizationReferences(references, TertiaryUsername);

    tertiaryEmployee.SetExternalKeys(
      references.Users[TertiaryUsername].Id,
      tertiaryReferences.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      tertiaryReferences.Charging.Id);

    var quaternaryEmployee = new Employee(
      firstName: "Ariana",
      lastName: "Reyes",
      middleName: "S",
      dateOfBirth: new DateTime(1987, 5, 28),
      gender: Gender.Female,
      civilStatus: CivilStatus.Married,
      bloodType: BloodType.ABNegative,
      status: Status.Active,
      height: 160m,
      weight: 58m,
      imageUrl: null,
      remarks: "Leads the human resources team",
      mealCredits: 6,
      tenant: Tenant.Maribago);

    quaternaryEmployee.SetContactInfo(new ContactInfo(
      email: "ariana.reyes@example.com",
      telNumber: "032-445-8899",
      mobileNumber: "0917-222-7788",
      address: "55 Coral Street, Lapu-Lapu",
      provincialAddress: "12 Garden Lane, Cebu",
      mothersMaidenName: "Sofia Santos",
      fathersName: "Ramon Reyes",
      emergencyContact: "Miguel Reyes",
      relationshipContact: "Husband",
      addressContact: "55 Coral Street, Lapu-Lapu",
      telNoContact: "032-445-7700",
      mobileNoContact: "0917-888-9900"));

    quaternaryEmployee.SetEducationInfo(new EducationInfo(
      educationalAttainment: EducationalAttainment.Bachelors,
      courseGraduated: "Business Administration",
      universityGraduated: "University of San Carlos"));

    quaternaryEmployee.SetEmploymentInfo(new EmploymentInfo(
      dateHired: new DateTime(2012, 4, 2),
      dateRegularized: new DateTime(2012, 10, 2),
      dateResigned: null,
      dateTerminated: null,
      tinNo: "667-889-001",
      sssNo: "12-3456789-0",
      hdmfNo: "HM-667788990",
      phicNo: "PH-667788990",
      bankAccount: "4433221100",
      hasServiceCharge: false));

    var quaternaryReferences = ResolveOrganizationReferences(references, QuaternaryUsername);

    quaternaryEmployee.SetExternalKeys(
      references.Users[QuaternaryUsername].Id,
      quaternaryReferences.Position.Id,
      references.Pay.Id,
      references.EmployeeType.Id,
      references.Level.Id,
      quaternaryReferences.Charging.Id);

    return new List<Employee> { primaryEmployee, secondaryEmployee, tertiaryEmployee, quaternaryEmployee };
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

  private record SeedReferenceData(IReadOnlyDictionary<string, AppUser> Users, Pay Pay, EmployeeType EmployeeType, Level Level, OrganizationSeedResult Organization);

  private sealed record AppUserSeedInfo(string Username, Credential Credential, Guid? SupervisedGroup, bool IsGlobalSupervisor);

  private sealed record DivisionSeed(string Division, IReadOnlyList<DepartmentSeed> Departments);

  private sealed record DepartmentSeed(string Department, IReadOnlyList<ChargingSeed> Chargings);

  private sealed record ChargingSeed(string Charging, IReadOnlyList<SectionSeed> Sections);

  private sealed record SectionSeed(string Section, IReadOnlyList<string> Positions);

  private sealed record EmployeeAssignment(string Division, string Department, string Charging, string Section, string Position);

  private sealed record EmployeeTypeSeedInfo(string Name, string Value);

  private sealed record LevelSeedInfo(string Name, string Value);

  private sealed record OrganizationSeedResult(
    IReadOnlyDictionary<string, Division> Divisions,
    IReadOnlyDictionary<string, Department> Departments,
    IReadOnlyDictionary<string, Charging> Chargings,
    IReadOnlyDictionary<string, Section> Sections,
    IReadOnlyDictionary<string, Position> Positions);
}
