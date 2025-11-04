using System;
using System.Collections.Generic;

namespace Bluewater.Web.Dashboard;

public record ChartSegmentRecord(string Label, decimal Value);

public record TrendPointRecord(string Label, decimal Value);

public record PerfectAttendanceLeaderRecord(
  Guid EmployeeId,
  string EmployeeName,
  int TotalTrackedDays,
  int PerfectDays,
  int LateDays,
  int Absences,
  decimal AttendanceRate);

public record HomeDashboardRecord(
  IReadOnlyList<ChartSegmentRecord> AttendanceSummary,
  IReadOnlyList<TrendPointRecord> WeeklyAttendanceTrend,
  IReadOnlyList<ChartSegmentRecord> LeaveDistribution,
  IReadOnlyList<PerfectAttendanceLeaderRecord> PerfectAttendanceLeaders,
  IReadOnlyList<TrendPointRecord> MonthlyAbsenceTrend,
  DateTime GeneratedAtUtc);
