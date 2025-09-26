using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Attendances;

public class AttendanceListAllRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  [Required]
  public string Charging { get; set; } = string.Empty;

  [Required]
  public DateOnly StartDate { get; set; }

  [Required]
  public DateOnly EndDate { get; set; }

  public Tenant Tenant { get; set; } = Tenant.Maribago;
}
