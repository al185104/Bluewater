namespace Bluewater.Web.Chargings;

public record ChargingRecord(Guid Id, string Name, string? Description, Guid? DepartmentId);
