using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class DashboardApiService(IApiClient apiClient) : IDashboardApiService
{
  private const string Route = "Dashboard/Home";

  public async Task<HomeDashboardSummary?> GetHomeDashboardAsync(
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    string requestUri = $"{Route}?tenant={tenant}";
    HomeDashboardResponseDto? response = await apiClient.GetAsync<HomeDashboardResponseDto>(requestUri, cancellationToken);

    return response?.Dashboard is null ? null : MapDashboard(response.Dashboard);
  }

  private static HomeDashboardSummary MapDashboard(HomeDashboardDto dto)
  {
    return new HomeDashboardSummary
    {
      AttendanceSummary = dto.AttendanceSummary?.Select(MapSegment).ToList() ?? new List<ChartSegmentSummary>(),
      WeeklyAttendanceTrend = dto.WeeklyAttendanceTrend?.Select(MapTrend).ToList() ?? new List<TrendPointSummary>(),
      LeaveDistribution = dto.LeaveDistribution?.Select(MapSegment).ToList() ?? new List<ChartSegmentSummary>(),
      PerfectAttendanceLeaders = dto.PerfectAttendanceLeaders?.Select(MapLeader).ToList() ?? new List<PerfectAttendanceLeaderboardEntry>(),
      MonthlyAbsenceTrend = dto.MonthlyAbsenceTrend?.Select(MapTrend).ToList() ?? new List<TrendPointSummary>(),
      GeneratedAtUtc = dto.GeneratedAtUtc
    };
  }

  private static ChartSegmentSummary MapSegment(ChartSegmentDto dto)
  {
    return new ChartSegmentSummary
    {
      Label = dto.Label ?? string.Empty,
      Value = dto.Value
    };
  }

  private static TrendPointSummary MapTrend(TrendPointDto dto)
  {
    return new TrendPointSummary
    {
      Label = dto.Label ?? string.Empty,
      Value = dto.Value
    };
  }

  private static PerfectAttendanceLeaderboardEntry MapLeader(PerfectAttendanceDto dto)
  {
    return new PerfectAttendanceLeaderboardEntry
    {
      EmployeeId = dto.EmployeeId,
      EmployeeName = dto.EmployeeName ?? string.Empty,
      TotalTrackedDays = dto.TotalTrackedDays,
      PerfectDays = dto.PerfectDays,
      LateDays = dto.LateDays,
      Absences = dto.Absences,
      AttendanceRate = dto.AttendanceRate
    };
  }
}
