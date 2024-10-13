using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Positions.List;
using Bluewater.UseCases.Sections;
using Bluewater.UseCases.Sections.List;
using Bluewater.UseCases.Departments;
using Bluewater.UseCases.Departments.List;
using Bluewater.UseCases.Divisions;
using Bluewater.UseCases.Divisions.List;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.List;
using Bluewater.UseCases.Holidays;
using Bluewater.UseCases.Holidays.List;
using Bluewater.UseCases.EmployeeTypes;
using Bluewater.UseCases.EmployeeTypes.List;
using Bluewater.UseCases.Levels;
using Bluewater.UseCases.Levels.List;

using MediatR;

namespace Bluewater.Server.Global;

public class GlobalService : IGlobalService
{
    public string ErrorMessage { get; set; } = "WEEEEE";
    // public IQueryable<DivisionDTO> Divisions { get; set; } = default!;
    // public IQueryable<DepartmentDTO> Departments { get; set; } = default!;
    // public IQueryable<SectionDTO> Sections { get; set; } = default!;
    // public IQueryable<PositionDTO> Positions { get; set; } = default!;
    // public IQueryable<ChargingDTO> Chargings { get; set; } = default!;
    // public IQueryable<HolidayDTO> Holidays { get; set; } = default!;
    // public IQueryable<EmployeeTypeDTO> EmployeeTypes { get; set; } = default!;
    // public IQueryable<LevelDTO> Levels { get; set; } = default!;
    public List<DivisionDTO> Divisions { get; set; } = new();
    public List<DepartmentDTO> Departments { get; set; } = new();
    public List<SectionDTO> Sections { get; set; } = new();
    public List<PositionDTO> Positions { get; set; } = new();
    public List<ChargingDTO> Chargings { get; set; } = new();
    public List<HolidayDTO> Holidays { get; set; } = new();
    public List<EmployeeTypeDTO> EmployeeTypes { get; set; } = new();
    public List<LevelDTO> Levels { get; set; } = new();

    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly IMediator Mediator;
    public GlobalService(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Mediator = mediator;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            List<Task> tasks = new();
            await LoadDivisions();
            await LoadDepartments();
            await LoadSections();
            await LoadPositions();
            await LoadChargings();
            await LoadHolidays();
            await LoadEmployeeTypes();
            await LoadLevels();

            await Task.WhenAll(tasks);            
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region Load Data
    private async Task LoadEmployeeTypes()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListEmployeeTypeQuery(null, null));
                if (result.IsSuccess)
                    EmployeeTypes = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadLevels()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListLevelQuery(null, null));
                if (result.IsSuccess)
                    Levels = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadHolidays()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListHolidayQuery(null, null));
                if (result.IsSuccess)
                    Holidays = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadChargings()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListChargingQuery(null, null));
                if (result.IsSuccess)
                    Chargings = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadPositions()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListPositionsQuery(null, null));
                if (result.IsSuccess)
                    Positions = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadSections()
    {
        try
        {
            using(var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListSectionsQuery(null, null));
                if (result.IsSuccess)
                    Sections = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadDepartments()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListDepartmentsQuery(null, null));
                if (result.IsSuccess)
                    Departments = result.Value.ToList();
            }
        }
        catch(Exception)
        {
            throw;
        }
    }

    private async Task LoadDivisions()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListDivisionsQuery(null, null));
                if (result.IsSuccess){
                    Divisions = result.Value.ToList();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion    
}