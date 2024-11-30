using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules;
public record EmployeeScheduleDTO()
{
    public Guid EmployeeId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;    
    public string Charging { get; set; } = string.Empty;
    public List<ShiftInfo> Shifts { get; set; } = new();
    public EmployeeScheduleDTO(Guid employeeId, string barcode, string name, string section, string charging, List<ShiftInfo> shifts) : this()
    {
        EmployeeId = employeeId;
        Barcode = barcode;
        Name = name;
        Shifts = shifts;
        Section = section;
        Charging = charging;
    }
}

public class ShiftInfo
{
    public Guid ScheduleId { get; set; }
    public ShiftDTO? Shift { get; set; }
    public DateOnly ScheduleDate { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsUpdated { get; set; } = false;
}