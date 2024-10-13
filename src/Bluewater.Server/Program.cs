using Microsoft.FluentUI.AspNetCore.Components;
using Bluewater.Server.Components;
using Bluewater.Server.Global;
using Ardalis.ListStartupServices;
using Ardalis.SharedKernel;
using Bluewater.Core.ContributorAggregate;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DivisionAggregate;
using Bluewater.UseCases.Contributors.Create;
using Bluewater.UseCases.Departments.Create;
using Bluewater.UseCases.Divisions.Create;
using Bluewater.Infrastructure;
using MediatR;
using System.Reflection;
using Serilog;
using Serilog.Extensions.Logging;
using Bluewater.Infrastructure.Email;
using Bluewater.Core.Interfaces;
using Bluewater.Core.SectionAggregate;
using Bluewater.UseCases.Sections.Create;
using Bluewater.Core.PositionAggregate;
using Bluewater.UseCases.Positions.Create;
using Bluewater.Core.ChargingAggregate;
using Bluewater.UseCases.Chargings.Create;
using Bluewater.Core.ShiftAggregate;
using Bluewater.UseCases.Shifts.Create;
using Bluewater.Core.HolidayAggregate;
using Bluewater.UseCases.Holidays.Create;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.UseCases.EmployeeTypes.Create;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.UseCases.Employees.Create;

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
    
    logger.Information("Starting web app");
    var microsoftLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

    ConfigureMediatR(builder);
    
    builder.Services.AddInfrastructureServices(builder.Configuration, microsoftLogger);
    builder.Services.AddSingleton<IGlobalService, GlobalService>();

    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      //builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

      AddShowAllServicesSupport(builder);
    }
    else
    {
      builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    builder.Services.AddFluentUIComponents();
    builder.Services.AddDataGridEntityFrameworkAdapter();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error", createScopeForErrors: true);
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    app.UseMigrationsEndPoint();
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
    };
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
    builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
  }

  static void AddShowAllServicesSupport(WebApplicationBuilder builder)
  {
    // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
    builder.Services.Configure<ServiceConfig>(config =>
    {
      config.Services = new List<ServiceDescriptor>(builder.Services);

      // optional - default path to view services is /listallservices - recommended to choose your own path
      config.Path = "/listservices";
    });
  }
}
