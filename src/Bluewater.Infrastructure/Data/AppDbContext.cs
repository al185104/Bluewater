using System.Reflection;
using Ardalis.SharedKernel;
using Bluewater.Core.ContributorAggregate;
using Bluewater.Core.DependentAggregate;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.LeaveCreditAggregate;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.PayAggregate;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.Core.Forms.UndertimeAggregate;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Forms.DeductionAggregate;
using Bluewater.Core.Forms.FailureInOutAggregate;
using Bluewater.Core.Forms.OtherEarningAggregate;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.ServiceChargeAggregate;

namespace Bluewater.Infrastructure.Data;
public class AppDbContext : DbContext
{
  private readonly IDomainEventDispatcher? _dispatcher;

  // DbSet Properties
  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<Employee> Employees => Set<Employee>();
  public DbSet<User> Users => Set<User>();
  public DbSet<Level> Levels => Set<Level>();
  public DbSet<EmployeeType> Types => Set<EmployeeType>();
  public DbSet<Position> Positions => Set<Position>();
  public DbSet<Section> Sections => Set<Section>();
  public DbSet<Department> Departments => Set<Department>();
  public DbSet<Division> Divisions => Set<Division>();
  public DbSet<Charging> Chargings => Set<Charging>();
  public DbSet<Shift> Shifts => Set<Shift>();
  public DbSet<Pay> Pays => Set<Pay>();
  public DbSet<LeaveCredit> LeaveCredits => Set<LeaveCredit>();
  public DbSet<Dependent> Dependents => Set<Dependent>();
  public DbSet<Holiday> Holidays => Set<Holiday>();
  public DbSet<Schedule> Schedules => Set<Schedule>();
  public DbSet<Timesheet> Timesheets => Set<Timesheet>();
  public DbSet<Attendance> Attendance => Set<Attendance>();

  // forms
  public DbSet<Overtime> Overtimes => Set<Overtime>();
  public DbSet<Undertime> Undertimes => Set<Undertime>();
  public DbSet<Leave> Leaves => Set<Leave>();
  public DbSet<Deduction> Deductions => Set<Deduction>();
  public DbSet<FailureInOut> FailureInOuts => Set<FailureInOut>();
  public DbSet<OtherEarning> OtherEarnings => Set<OtherEarning>();
  public DbSet<Payroll> Payrolls => Set<Payroll>();
  public DbSet<ServiceCharge> ServiceCharges => Set<ServiceCharge>();

  public AppDbContext(DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher;
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
      optionsBuilder.UseSqlite("Data Source=localdb_sit01.db");
    else
      base.OnConfiguring(optionsBuilder);
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
