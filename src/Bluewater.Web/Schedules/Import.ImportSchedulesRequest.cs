using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Schedules;

public class ImportSchedulesRequest
{
  public const string Route = "/Schedules/Import";

  public Tenant Tenant { get; set; } = Tenant.Maribago;
  public List<ImportScheduleEntryRequest> Entries { get; set; } = [];
}

public class ImportScheduleEntryRequest
{
  public string Barcode { get; set; } = string.Empty;
  public DateOnly ScheduleDate { get; set; }
  public string? ShiftName { get; set; }
  public bool IsDefault { get; set; }
}
