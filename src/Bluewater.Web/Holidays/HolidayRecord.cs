namespace Bluewater.Web.Holidays;

public record HolidayRecord(Guid Id, string Name, string? Description, DateTime Date, bool IsRegular);
