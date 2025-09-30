using System.Reflection;
using Ardalis.ListStartupServices;
using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.ContributorAggregate;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;
using Bluewater.Core.LevelAggregate;
using Bluewater.Infrastructure;
using Bluewater.Infrastructure.Data;
using Bluewater.Infrastructure.Email;
using Bluewater.UseCases.Chargings.Create;
using Bluewater.UseCases.Contributors.Create;
using Bluewater.UseCases.Departments.Create;
using Bluewater.UseCases.Divisions.Create;
using Bluewater.UseCases.Employees.Create;
using Bluewater.UseCases.EmployeeTypes.Create;
using Bluewater.UseCases.Holidays.Create;
using Bluewater.UseCases.LeaveCredits.Create;
using Bluewater.UseCases.Leaves.Create;
using Bluewater.UseCases.Levels.Create;
using FastEndpoints;
using FastEndpoints.Swagger;
using MediatR;
using Serilog;
using Serilog.Extensions.Logging;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
var microsoftLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

// Configure Web Behavior
builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

ConfigureMediatR();

builder.Services.AddInfrastructureServices(builder.Configuration, microsoftLogger);

if (builder.Environment.IsDevelopment())
{
  // Use a local test email server
  // See: https://ardalis.com/configuring-a-local-test-email-server/
  builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();

  // Otherwise use this:
  //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
  AddShowAllServicesSupport();
}
else
{
  builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
}
else
{
  app.UseDefaultExceptionHandler(); // from FastEndpoints
  app.UseHsts();
}

app.UseFastEndpoints()
    .UseSwaggerGen(); // Includes AddFileServer and static files middleware

//app.UseHttpsRedirection();

await SeedDatabase(app);

app.Run();

static async Task SeedDatabase(WebApplication app)
{
  using var scope = app.Services.CreateScope();
  var services = scope.ServiceProvider;

  try
  {
    var context = services.GetRequiredService<AppDbContext>();
    //          context.Database.Migrate();
    context.Database.EnsureCreated();
    await ShiftDataSeeder.SeedAsync(context);
    await EmployeeDataSeeder.SeedAsync(context);
    await ScheduleDataSeeder.SeedAsync(context);
    await TimesheetDataSeeder.SeedAsync(context);
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  }
}

void ConfigureMediatR()
{
  var mediatRAssemblies = new[]
{
  Assembly.GetAssembly(typeof(Contributor)), // Core
  Assembly.GetAssembly(typeof(CreateContributorCommand)), // UseCases

  Assembly.GetAssembly(typeof(Division)), // Core
  Assembly.GetAssembly(typeof(CreateDivisionCommand)), // UseCases

  Assembly.GetAssembly(typeof(Department)), // Core
  Assembly.GetAssembly(typeof(CreateDepartmentCommand)), // UseCases

  Assembly.GetAssembly(typeof(Employee)), // Core
  Assembly.GetAssembly(typeof(CreateEmployeeCommand)), // UseCases

  Assembly.GetAssembly(typeof(EmployeeType)), // Core
  Assembly.GetAssembly(typeof(CreateEmployeeTypeCommand)), // UseCases

  Assembly.GetAssembly(typeof(Charging)), // Core
  Assembly.GetAssembly(typeof(CreateChargingCommand)), // UseCases

  Assembly.GetAssembly(typeof(Holiday)), // Core
  Assembly.GetAssembly(typeof(CreateHolidayCommand)), // UseCases

  Assembly.GetAssembly(typeof(LeaveCredit)), // Core
  Assembly.GetAssembly(typeof(CreateLeaveCreditCommand)), // UseCases

  Assembly.GetAssembly(typeof(Leave)), // Core
  Assembly.GetAssembly(typeof(CreateLeaveCommand)), // UseCases

  Assembly.GetAssembly(typeof(Level)), // Core
  Assembly.GetAssembly(typeof(CreateLevelCommand)) // UseCases
};
  builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
  builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
  builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
}

void AddShowAllServicesSupport()
{
  // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
  builder.Services.Configure<ServiceConfig>(config =>
  {
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
  });
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
