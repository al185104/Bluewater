using System;

namespace Bluewater.App.Models;

public record EmployeeExternalOption(Guid? Id, string Label, bool IsPlaceholder = false)
{
  public static EmployeeExternalOption CreateEmpty(string label) => new(null, label, true);

  public override string ToString() => Label;
}
