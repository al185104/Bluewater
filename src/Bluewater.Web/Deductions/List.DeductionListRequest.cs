using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Deductions;

public class DeductionListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  public Tenant Tenant { get; set; } = Tenant.Maribago;

  public Guid? ChargingId { get; set; }
}
