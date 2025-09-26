using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Leaves;

public class LeaveListRequest
{
  public int? Skip { get; set; }
  public int? Take { get; set; }
  public Tenant Tenant { get; set; } = Tenant.Maribago;
}
