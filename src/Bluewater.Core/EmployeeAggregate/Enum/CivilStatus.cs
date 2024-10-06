using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum CivilStatus
{
  [Description("Not Set")]
  NotSet,
  [Description("Single")]
  Single,
  [Description("Married")]
  Married,
  [Description("Widow")]
  Widow,
  [Description("Annulled")]
  Annulled
}
