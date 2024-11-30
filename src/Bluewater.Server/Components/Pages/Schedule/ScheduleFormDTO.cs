using Bluewater.UseCases.Schedules;

namespace Bluewater.Server.Components.Pages.Schedule;

//public record ScheduleFormDTO(Guid EmployeeId, string Name, ShiftInfo? SundayShift, ShiftInfo? MondayShift, ShiftInfo? TuesdayShift, ShiftInfo? WednesdayShift, ShiftInfo? ThursdayShift, ShiftInfo? FridayShift, ShiftInfo? SaturdayShift);
public record class ScheduleFormDTO
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ShiftInfo? SundayShift { get; set; }
    public ShiftInfo? MondayShift { get; set; }
    public ShiftInfo? TuesdayShift { get; set; }
    public ShiftInfo? WednesdayShift { get; set; }
    public ShiftInfo? ThursdayShift { get; set; }
    public ShiftInfo? FridayShift { get; set; }
    public ShiftInfo? SaturdayShift { get; set; }

    // Default constructor for initialization
    public ScheduleFormDTO() { }

    // Parameterized constructor for convenience
    public ScheduleFormDTO(
        Guid employeeId,
        string name,
        ShiftInfo? sundayShift,
        ShiftInfo? mondayShift,
        ShiftInfo? tuesdayShift,
        ShiftInfo? wednesdayShift,
        ShiftInfo? thursdayShift,
        ShiftInfo? fridayShift,
        ShiftInfo? saturdayShift)
    {
        EmployeeId = employeeId;
        Name = name;
        SundayShift = sundayShift;
        MondayShift = mondayShift;
        TuesdayShift = tuesdayShift;
        WednesdayShift = wednesdayShift;
        ThursdayShift = thursdayShift;
        FridayShift = fridayShift;
        SaturdayShift = saturdayShift;
    }
}


