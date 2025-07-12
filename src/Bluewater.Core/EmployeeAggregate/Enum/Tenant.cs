using System.ComponentModel;

namespace Bluewater.Core.EmployeeAggregate.Enum;
public enum Tenant
{
  [Description("Bluewater Maribago")]
  Maribago = 0,
  [Description("Bluewater Panglao")]
  Panglao,
  [Description("Bluewater Sumilon")]
  Sumilon,
  [Description("Almont Inland Hotel")]
  AlmontInland,
  [Description("Almont City Hotel")]
  AlmontCity,
  [Description("Almont Beach Resort")]
  AlmontBeach
}
