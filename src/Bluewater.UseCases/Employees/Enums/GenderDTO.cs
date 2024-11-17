using System.ComponentModel;

namespace Bluewater.UseCases.Employees.Enums;

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
