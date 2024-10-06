using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum EducationalAttainment
{
  [Description("Not Set")]
  NotSet,
  [Description("None")]
  None,
  [Description("Elementary")]
  Elementary,
  [Description("High School")]
  HighSchool,
  [Description("Diploma/Vocational")]
  Diploma_Vocational,
  [Description("College")]
  College,
  [Description("Bachelors")]
  Bachelors,
  [Description("Masters")]
  Masters,
  [Description("Doctorate")]
  Doctorate,
  [Description("Others")]
  Others
}
