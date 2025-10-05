using Bluewater.App.Models;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Selectors;

public class AlternateEmployeeTemplateSelector : DataTemplateSelector
{
  public DataTemplate? PrimaryTemplate { get; set; }

  public DataTemplate? AlternateTemplate { get; set; }

  protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
  {
    if (item is not EmployeeSummary employee)
    {
      return PrimaryTemplate ?? AlternateTemplate ?? new DataTemplate();
    }

    if (employee.RowIndex % 2 == 1 && AlternateTemplate is not null)
    {
      return AlternateTemplate;
    }

    return PrimaryTemplate ?? AlternateTemplate ?? new DataTemplate();
  }
}
