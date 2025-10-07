using System;

namespace Bluewater.App.Models;

public class PositionSummary : IRowIndexed
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public Guid SectionId { get; set; }
  public int RowIndex { get; set; }
}
