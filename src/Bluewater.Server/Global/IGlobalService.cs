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
using Bluewater.UseCases.Shifts;

namespace Bluewater.Server.Global;

public interface IGlobalService
{
    string ErrorMessage { get; set; }
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
}