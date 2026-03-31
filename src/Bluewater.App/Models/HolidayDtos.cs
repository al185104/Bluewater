using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class HolidayListResponseDto
{
  public List<HolidayDto?> Holidays { get; set; } = new();
}

public class HolidayDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime Date { get; set; }
  public bool IsRegular { get; set; }
}

public class CreateHolidayRequestDto
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime Date { get; set; }
  public bool IsRegular { get; set; } = true;
}

public class UpdateHolidayRequestDto
{
  public Guid HolidayId { get; set; }
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime Date { get; set; }
  public bool IsRegular { get; set; }

  public static string BuildRoute(Guid holidayId) => $"Holidays/{holidayId}";
}

public class UpdateHolidayResponseDto
{
  public HolidayDto Holiday { get; set; } = new();
}
