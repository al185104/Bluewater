using System.Reflection;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.Timelog.Components;
using Bluewater.UseCases.Timesheets.Create;
using Bluewater.Infrastructure;
using MediatR;
using Serilog;
using Serilog.Extensions.Logging;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.ContributorAggregate;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.PositionAggregate;
using Bluewater.UseCases.Attendances.Create;
using Bluewater.UseCases.Chargings.Create;
using Bluewater.UseCases.Contributors.Create;
using Bluewater.UseCases.Departments.Create;
using Bluewater.UseCases.Divisions.Create;
using Bluewater.UseCases.Employees.Create;
using Bluewater.UseCases.EmployeeTypes.Create;
using Bluewater.UseCases.Holidays.Create;
using Bluewater.UseCases.Positions.Create;
using Bluewater.UseCases.Schedules.Create;
using Bluewater.UseCases.Sections.Create;
using Bluewater.UseCases.Shifts.Create;
using Bluewater.UseCases.Users.Create;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.AttendanceAggregate;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

    //add a logger for the application
    var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateLogger();

    var microsoftLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

    ConfigureMediatR(builder);
    builder.Services.AddInfrastructureServices(builder.Configuration, microsoftLogger);
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error", createScopeForErrors: true);
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
  }

  static void ConfigureMediatR(WebApplicationBuilder builder)
  {
    var mediatRAssemblies = new[]
    {
      Assembly.GetAssembly(typeof(Contributor)), // Core
      Assembly.GetAssembly(typeof(CreateContributorCommand)), // UseCases

      Assembly.GetAssembly(typeof(Division)), // Core
      Assembly.GetAssembly(typeof(CreateDivisionCommand)), // UseCases

      Assembly.GetAssembly(typeof(Department)), // Core
      Assembly.GetAssembly(typeof(CreateDepartmentCommand)), // UseCases

      Assembly.GetAssembly(typeof(Section)), // Core
      Assembly.GetAssembly(typeof(CreateSectionCommand)), // UseCases

      Assembly.GetAssembly(typeof(Position)), // Core
      Assembly.GetAssembly(typeof(CreatePositionCommand)), // UseCases

      Assembly.GetAssembly(typeof(Charging)), // Core
      Assembly.GetAssembly(typeof(CreateChargingCommand)), // UseCases

      Assembly.GetAssembly(typeof(Shift)),
      Assembly.GetAssembly(typeof(CreateShiftCommand)),

      Assembly.GetAssembly(typeof(Holiday)),
      Assembly.GetAssembly(typeof(CreateHolidayCommand)),

      Assembly.GetAssembly(typeof(EmployeeType)),
      Assembly.GetAssembly(typeof(CreateEmployeeTypeCommand)),

      Assembly.GetAssembly(typeof(Employee)),
      Assembly.GetAssembly(typeof(CreateEmployeeCommand)),

      Assembly.GetAssembly(typeof(Schedule)),
      Assembly.GetAssembly(typeof(CreateScheduleCommand)),

      Assembly.GetAssembly(typeof(Timesheet)),
      Assembly.GetAssembly(typeof(CreateTimesheetCommand)),

      Assembly.GetAssembly(typeof(AppUser)),
      Assembly.GetAssembly(typeof(CreateUserCommand)),

      Assembly.GetAssembly(typeof(Attendance)),
      Assembly.GetAssembly(typeof(CreateAttendanceCommand)),

      Assembly.GetAssembly(typeof(Leave))

    };
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
    builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
  }

}
