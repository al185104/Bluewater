using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum Gender
{
  [Description("Not Set")]
  NotSet,
  [Description("Male")]
  Male,
  [Description("Female")]
  Female,
  [Description("Other")]
  Other
}
