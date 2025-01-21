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
using Bluewater.Core.ScheduleAggregate;
using Bluewater.UseCases.Schedules.Create;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UseCases.Timesheets.Create;
using Bluewater.Core.UserAggregate;
using Bluewater.UseCases.Users.Create;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.UseCases.Attendances.Create;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Bluewater.Infrastructure.Data;
using System.Security.Claims;
using Bluewater.Server.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
    builder.Services.AddRazorPages();

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Persist Data Protection keys to a folder
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys")))
        .SetApplicationName("Bluewater");

    //add a logger for the application
    var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateLogger();
    
    var microsoftLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

    ConfigureMediatR(builder);
    
    builder.Services.AddInfrastructureServices(builder.Configuration, microsoftLogger);
    builder.Services.AddSingleton<IGlobalService, GlobalService>();

    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
      options.Password.RequireDigit = false;
      options.Password.RequiredLength = 5;
      options.Password.RequireLowercase = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireNonAlphanumeric = false;
      options.SignIn.RequireConfirmedEmail = false;
      options.SignIn.RequireConfirmedAccount = false;
    }).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
    

    builder.Services.AddFluentUIComponents();
    builder.Services.AddDataGridEntityFrameworkAdapter();
    builder.Services.AddScoped<IEmployeeAuthService, EmployeeAuthService>();
    builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
    // builder.Services.AddScoped<CustomAuthenticationStateProvider>();
    // builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
  
    if (builder.Environment.IsDevelopment())
    {
      builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
      AddShowAllServicesSupport(builder);
    }
    else
    {
      builder.Services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }


    //new
    // builder.Services.AddHttpContextAccessor();
    // builder.Services.AddRazorPages(options =>
    // {
    //     options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
    // });
    // builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    // .AddCookie(options =>
    // {
    //     options.LoginPath = "/Login"; // Redirect if unauthenticated
    // });    
    // builder.Services.ConfigureApplicationCookie(options =>
    // {
    //     options.LoginPath = "/Login"; // Ensure it matches your route
    // });   
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();

    builder.Services.AddAuthorizationCore(); // Adds support for the [Authorize] attribute


    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; // Ensure roles are mapped correctly
    });

    var app = builder.Build();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    
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

    app.MapRazorPages();
    //app.MapBlazorHub();

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

// var connectionString = builder.Configuration.GetConnectionString("IdentityDataContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDataContextConnection' not found.");

// builder.Services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(connectionString));

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityDataContext>();
