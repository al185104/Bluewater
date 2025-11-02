using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Extensions;

public static class RowIndexExtensions
{
  public static void UpdateRowIndexes<T>(this IList<T> items)
    where T : IRowIndexed
  {
    for (int i = 0; i < items.Count; i++)
    {
      if (items[i] is null)
      {
        continue;
      }

      items[i].RowIndex = i;
    }
  }
}
