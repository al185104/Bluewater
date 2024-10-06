using System.ComponentModel;

namespace Bluewater.Core.UserAggregate.Enum;
public enum Credential
{
  [Description("None")]
  None = 0,
  [Description("Probationary")]
  Probationary,
  [Description("Employee")]
  Employee,
  [Description("Scheduler")]
  Scheduler,
  [Description("Payroll")]
  Payroll,
  [Description("Manager")]
  Manager,
  [Description("Supervisor")]
  Supervisor,
  [Description("Admin")]
  Admin,
  [Description("SuperAdmin")]
  SuperAdmin
}
