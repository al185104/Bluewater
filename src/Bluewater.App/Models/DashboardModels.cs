using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class HomeDashboardSummary
{
  public List<ChartSegmentSummary> AttendanceSummary { get; set; } = new();

  public List<TrendPointSummary> WeeklyAttendanceTrend { get; set; } = new();

  public List<ChartSegmentSummary> LeaveDistribution { get; set; } = new();

  public List<PerfectAttendanceLeaderboardEntry> PerfectAttendanceLeaders { get; set; } = new();

  public List<TrendPointSummary> MonthlyAbsenceTrend { get; set; } = new();

  public DateTime GeneratedAtUtc { get; set; }
}

public class ChartSegmentSummary
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }
}

public class TrendPointSummary
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }
}

public class PerfectAttendanceLeaderboardEntry
{
  public Guid EmployeeId { get; set; }

  public string EmployeeName { get; set; } = string.Empty;

  public int TotalTrackedDays { get; set; }

  public int PerfectDays { get; set; }

  public int LateDays { get; set; }

  public int Absences { get; set; }

  public double AttendanceRate { get; set; }
}
