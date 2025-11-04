using System.Collections.Generic;
using System.Linq;
using Bluewater.UseCases.Dashboards.Home;

namespace Bluewater.Web.Dashboard;

public static class HomeDashboardMapper
{
  public static HomeDashboardRecord ToRecord(HomeDashboardDTO dto)
  {
    return new HomeDashboardRecord(
      dto.AttendanceSummary.Select(ToRecord).ToList(),
      dto.WeeklyAttendanceTrend.Select(ToRecord).ToList(),
      dto.LeaveDistribution.Select(ToRecord).ToList(),
      dto.PerfectAttendanceLeaders.Select(ToRecord).ToList(),
      dto.MonthlyAbsenceTrend.Select(ToRecord).ToList(),
      dto.GeneratedAtUtc);
  }

  private static ChartSegmentRecord ToRecord(ChartSegmentDTO dto) => new(dto.Label, dto.Value);

  private static TrendPointRecord ToRecord(TrendPointDTO dto) => new(dto.Label, dto.Value);

  private static PerfectAttendanceLeaderRecord ToRecord(PerfectAttendanceLeaderDTO dto) =>
    new(dto.EmployeeId, dto.EmployeeName, dto.TotalTrackedDays, dto.PerfectDays, dto.LateDays, dto.Absences, dto.AttendanceRate);
}
