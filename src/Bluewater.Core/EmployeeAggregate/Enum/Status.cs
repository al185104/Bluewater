using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum Status
{
  [Description("Not Set")]
  NotSet,
  [Description("Active")]
  Active,
  [Description("Inactive")]
  Inactive,
  [Description("On Leave")]
  OnLeave,
  [Description("Resigned")]
  Resigned,
  [Description("Terminated")]
  Terminated
}
