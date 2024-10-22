using Bluewater.UseCases.Schedules;

namespace Bluewater.Server.Components.Pages.Schedule;

public record ScheduleFormDTO(Guid EmployeeId, string Name, ShiftInfo? SundayShift, ShiftInfo? MondayShift, ShiftInfo? TuesdayShift, ShiftInfo? WednesdayShift, ShiftInfo? ThursdayShift, ShiftInfo? FridayShift, ShiftInfo? SaturdayShift);