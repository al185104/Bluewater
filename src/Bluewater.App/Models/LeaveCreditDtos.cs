using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class LeaveCreditListResponseDto
{
  public List<LeaveCreditDto?> LeaveCredits { get; set; } = new();
}

public class LeaveCreditDto
{
  public Guid Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal DefaultCredits { get; set; }
  public int SortOrder { get; set; }
  public bool IsLeaveWithPay { get; set; }
  public bool IsCanCarryOver { get; set; }
}
