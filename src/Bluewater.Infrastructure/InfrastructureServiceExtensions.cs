using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.Services;
using Bluewater.Infrastructure.Data;
using Bluewater.Infrastructure.Data.Queries;
using Bluewater.Infrastructure.Email;
using Bluewater.UseCases.Chargings.List;
using Bluewater.UseCases.Contributors.List;
using Bluewater.UseCases.Departments.List;
using Bluewater.UseCases.Divisions.List;
using Bluewater.UseCases.Positions.List;
using Bluewater.UseCases.Sections.List;
using Bluewater.UseCases.Shifts.List;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bluewater.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    // if development use sqlite, else use sql server
    string connectionString = string.Empty;
    if (config.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development"){
      connectionString = config.GetConnectionString("SqliteConnection")!;
      Guard.Against.Null(connectionString);

      services.AddDbContext<AppDbContext>(options =>
      options.UseSqlite(connectionString)
      .LogTo(Console.WriteLine, LogLevel.None));

    }
    else{
      connectionString = config.GetConnectionString("DefaultConnection")!;
      Guard.Against.Null(connectionString);
      services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(connectionString));
    }

    // string? connectionString = config.GetConnectionString("DefaultConnection");
    // string? connectionString = config.GetConnectionString("SqliteConnection");        
    // services.AddDbContext<AppDbContext>(options =>
    // options.UseSqlite(connectionString)
    // .LogTo(Console.WriteLine, LogLevel.None));
     //options.UseSqlServer(connectionString));

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    // Contributors
    services.AddScoped<IListContributorsQueryService, ListContributorsQueryService>();
    services.AddScoped<IDeleteContributorService, DeleteContributorService>();
    // Divisions
    services.AddScoped<IListDivisionsQueryService, ListDivisionsQueryService>();
    services.AddScoped<IDeleteDivisionService, DeleteDivisionService>();
    // Departments
    services.AddScoped<IListDepartmentsQueryService, ListDepartmentsQueryService>();
    services.AddScoped<IDeleteDepartmentService, DeleteDepartmentService>();
    // Sections
    services.AddScoped<IListSectionsQueryService, ListSectionsQueryService>();
    services.AddScoped<IDeleteSectionService, DeleteSectionService>();
    // Positions
    services.AddScoped<IListPositionsQueryService, ListPositionsQueryService>();
    services.AddScoped<IDeletePositionService, DeletePositionService>();
    // Chargings
    services.AddScoped<IListChargingQueryService, ListChargingQueryService>();
    services.AddScoped<IDeleteChargingService, DeleteChargingService>();
    // Holidays
    services.AddScoped<IDeleteHolidayService, DeleteHolidayService>();
    // Employee Types
    services.AddScoped<IDeleteEmployeeTypeService, DeleteEmployeeTypeService>();
    // Employee Levels
    services.AddScoped<IDeleteLevelService, DeleteLevelService>();
    // Shifts
    services.AddScoped<IListShiftQueryService, ListShiftsQueryService>();
    // Employee
    services.AddScoped<IDeleteEmployeeService, DeleteEmployeeService>();

    services.AddScoped<IEmailSender, FakeEmailSender>();

    services.Configure<MailserverConfiguration>(config.GetSection("Mailserver"));

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
