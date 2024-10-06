using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum BloodType
{
  [Description("Not Set")]
  NotSet,
  [Description("A+")]
  APositive,
  [Description("A-")]
  ANegative,
  [Description("B+")]
  BPositive,
  [Description("B-")]
  BNegative,
  [Description("AB+")]
  ABPositive,
  [Description("AB-")]
  ABNegative,
  [Description("O+")]
  OPositive,
  [Description("O-")]
  ONegative
}

