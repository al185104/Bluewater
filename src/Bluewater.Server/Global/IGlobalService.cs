using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Sections;
using Bluewater.UseCases.Departments;
using Bluewater.UseCases.Divisions;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Holidays;
using Bluewater.UseCases.EmployeeTypes;
using Bluewater.UseCases.Levels;
using Bluewater.UseCases.Shifts;

namespace Bluewater.Server.Global;

public interface IGlobalService
{
    public List<DivisionDTO> Divisions { get; set; }
    public List<DepartmentDTO> Departments { get; set; }
    public List<SectionDTO> Sections { get; set; }
    public List<PositionDTO> Positions { get; set; }
    public List<ChargingDTO> Chargings { get; set; } 
    public List<HolidayDTO> Holidays { get; set; } 
    public List<EmployeeTypeDTO> EmployeeTypes { get; set; } 
    public List<LevelDTO> Levels { get; set; } 
    public List<ShiftDTO> Shifts { get; set; }

    Task LoadDataAsync();
    (DateOnly startDate, DateOnly endDate) GetStartDateAndEndDateOfWeekByDate(DateTime date);
    (DateOnly startDate, DateOnly endDate) GetStartDateAndEndDateOfPayslip(DateOnly date);
    (decimal dailyRate, decimal hourlyRate) GetRatesByEmployeeType(decimal basicRate, bool isRegular);
}