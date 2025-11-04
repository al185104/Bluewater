using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class HomeDashboardResponseDto
{
  public HomeDashboardDto? Dashboard { get; set; }
}

public class HomeDashboardDto
{
  public List<ChartSegmentDto> AttendanceSummary { get; set; } = new();

  public List<TrendPointDto> WeeklyAttendanceTrend { get; set; } = new();

  public List<ChartSegmentDto> LeaveDistribution { get; set; } = new();

  public List<PerfectAttendanceDto> PerfectAttendanceLeaders { get; set; } = new();

  public List<TrendPointDto> MonthlyAbsenceTrend { get; set; } = new();

  public DateTime GeneratedAtUtc { get; set; }
}

public class ChartSegmentDto
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }
}

public class TrendPointDto
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }
}

public class PerfectAttendanceDto
{
  public Guid EmployeeId { get; set; }

  public string EmployeeName { get; set; } = string.Empty;

  public int TotalTrackedDays { get; set; }

  public int PerfectDays { get; set; }

  public int LateDays { get; set; }

  public int Absences { get; set; }

  public double AttendanceRate { get; set; }
}
