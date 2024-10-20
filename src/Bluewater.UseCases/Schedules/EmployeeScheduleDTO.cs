using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules;
public record EmployeeScheduleDTO(Guid EmployeeId, string Name, List<ShiftInfo> Shifts);

public class ShiftInfo
{
    public ShiftDTO Shift { get; set; } = null!;
    public DateOnly ScheduleDate { get; set; }
    public bool IsDefault { get; set; }
}