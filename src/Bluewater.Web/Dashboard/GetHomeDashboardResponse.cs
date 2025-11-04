using System;

namespace Bluewater.Web.Dashboard;

public class GetHomeDashboardResponse
{
  public HomeDashboardRecord Dashboard { get; set; } = new(
    Array.Empty<ChartSegmentRecord>(),
    Array.Empty<TrendPointRecord>(),
    Array.Empty<ChartSegmentRecord>(),
    Array.Empty<PerfectAttendanceLeaderRecord>(),
    Array.Empty<TrendPointRecord>(),
    DateTime.UtcNow);
}
