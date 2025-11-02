namespace Bluewater.UseCases.Schedules;

using System.ComponentModel.DataAnnotations;

public class ScheduleImportDTO
{
    [Display(Name = "ID")]
    public string Id { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string ScheduleDate { get; init; } = string.Empty;

    [Display(Name = "Date")]
    public string Date { get; init; } = string.Empty;

    public string Shift { get; init; } = string.Empty;

    public string ShiftCode { get; init; } = string.Empty;

    public string? IsDefault { get; init; }

    public string? Default { get; init; }

    public string EmployeeIdentifier => string.IsNullOrWhiteSpace(Username) ? Id : Username;

    public string ShiftIdentifier => string.IsNullOrWhiteSpace(Shift) ? ShiftCode : Shift;

    public string ScheduleDateText => string.IsNullOrWhiteSpace(ScheduleDate) ? Date : ScheduleDate;
}
