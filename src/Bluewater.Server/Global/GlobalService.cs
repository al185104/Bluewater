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
using Bluewater.UseCases.Shifts.List;
using Bluewater.UseCases.Shifts;

namespace Bluewater.Server.Global;

public class GlobalService : IGlobalService
{
    public List<DivisionDTO> Divisions { get; set; } = new();
    public List<DepartmentDTO> Departments { get; set; } = new();
    public List<SectionDTO> Sections { get; set; } = new();
    public List<PositionDTO> Positions { get; set; } = new();
    public List<ChargingDTO> Chargings { get; set; } = new();
    public List<HolidayDTO> Holidays { get; set; } = new();
    public List<EmployeeTypeDTO> EmployeeTypes { get; set; } = new();
    public List<LevelDTO> Levels { get; set; } = new();
    public List<ShiftDTO> Shifts { get; set; } = new();

    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly IMediator Mediator;
    public GlobalService(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Mediator = mediator;
    }

    # region Public Methods
    public async Task LoadDataAsync()
    {
        try
        {
            if (Divisions.Count > 0) return;

            List<Task> tasks = new(){
                LoadDivisions(),
                LoadDepartments(),
                LoadSections(),
                LoadPositions(),
                LoadChargings(),
                LoadHolidays(),
                LoadEmployeeTypes(),
                LoadLevels(),
                LoadShifts()
            };

            await Task.WhenAll(tasks);            
        }
        catch (Exception)
        {
            throw;
        }
    }

    public (DateOnly startDate, DateOnly endDate) GetStartDateAndEndDateOfWeekByDate(DateTime date)
    {
        var today = DateOnly.FromDateTime(date);
        var dayOfWeek = (int)today.DayOfWeek;
        var startDate = today.AddDays(-dayOfWeek);
        var endDate = startDate.AddDays(6);
        return (startDate, endDate);
    }    
    #endregion

    #region Load Data
    private async Task LoadShifts()
    {
        try
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ListShiftQuery(null, null));
                if (result.IsSuccess)
                    Shifts = result.Value.ToList();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

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

    public (DateOnly startDate, DateOnly endDate) GetStartDateAndEndDateOfPayslip(DateOnly date)
    {
        DateOnly start;
        DateOnly end;

        if (date.Day >= 11 && date.Day <= 25)
        {
            start = new DateOnly(date.Year, date.Month, 11);
            end = new DateOnly(date.Year, date.Month, 25);
        }
        else if (date.Day < 11)
        {
            if (date.Month == 1)
            {
                start = new DateOnly(date.Year - 1, 12, 26);
                end = new DateOnly(date.Year, date.Month, 10);
            }
            else
            {
                start = new DateOnly(date.Year, date.Month - 1, 26);
                end = new DateOnly(date.Year, date.Month, 10);
            }
        }
        else
        {
            start = new DateOnly(date.Year, date.Month, 26);
            end = new DateOnly(date.Year, date.Month + 1, 10);
        }

        return (start, end);
    }

    public (decimal dailyRate, decimal hourlyRate) GetRatesByEmployeeType(decimal basicRate, bool isRegular)
    {
        decimal dailyRate = 0;
        if (isRegular)
            dailyRate = Math.Round(basicRate / 26, 2);
        else
            dailyRate = Math.Round(basicRate / (decimal)30.41, 2);

        return (dailyRate, Math.Round(dailyRate / 8, 2));
    }
}