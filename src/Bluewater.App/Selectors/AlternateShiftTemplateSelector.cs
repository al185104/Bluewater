using Bluewater.App.Models;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Selectors;

public class AlternateShiftTemplateSelector : DataTemplateSelector
{
  public DataTemplate? PrimaryTemplate { get; set; }

  public DataTemplate? AlternateTemplate { get; set; }

  protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
  {
    if (item is not IRowIndexed rowIndexed)
    {
      return PrimaryTemplate ?? AlternateTemplate ?? new DataTemplate();
    }

    if (rowIndexed.RowIndex % 2 == 1 && AlternateTemplate is not null)
    {
      return AlternateTemplate;
    }

    return PrimaryTemplate ?? AlternateTemplate ?? new DataTemplate();
  }
}
