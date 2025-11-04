using System;
using System.Collections.Generic;

namespace Bluewater.UseCases.Dashboards.Home;

public record ChartSegmentDTO(string Label, decimal Value);

public record TrendPointDTO(string Label, decimal Value);

public record PerfectAttendanceLeaderDTO(
  Guid EmployeeId,
  string EmployeeName,
  int TotalTrackedDays,
  int PerfectDays,
  int LateDays,
  int Absences,
  decimal AttendanceRate);

public class HomeDashboardDTO
{
  public List<ChartSegmentDTO> AttendanceSummary { get; init; } = new();

  public List<TrendPointDTO> WeeklyAttendanceTrend { get; init; } = new();

  public List<ChartSegmentDTO> LeaveDistribution { get; init; } = new();

  public List<PerfectAttendanceLeaderDTO> PerfectAttendanceLeaders { get; init; } = new();

  public List<TrendPointDTO> MonthlyAbsenceTrend { get; init; } = new();

  public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}
