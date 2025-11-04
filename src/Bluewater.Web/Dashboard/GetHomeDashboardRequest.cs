using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Dashboard;

public class GetHomeDashboardRequest
{
  [Required]
  public Tenant Tenant { get; set; } = Tenant.Maribago;
}
