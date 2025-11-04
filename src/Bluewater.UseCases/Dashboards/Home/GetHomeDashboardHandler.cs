using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Forms.LeaveAggregate.Specifications;

namespace Bluewater.UseCases.Dashboards.Home;

internal class GetHomeDashboardHandler(
  IReadRepository<Attendance> attendanceRepository,
  IReadRepository<Leave> leaveRepository) : IQueryHandler<GetHomeDashboardQuery, Result<HomeDashboardDTO>>
{
  private const int WeeklyWindowDays = 6;
  private const int PerfectAttendanceWindowDays = 29;
  private const int LeaveDistributionWindowMonths = 3;
  private const int MonthlyTrendWindowMonths = 11;

  public async Task<Result<HomeDashboardDTO>> Handle(GetHomeDashboardQuery request, CancellationToken cancellationToken)
  {
    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
    DateOnly weeklyStart = today.AddDays(-WeeklyWindowDays);
    DateOnly perfectStart = today.AddDays(-PerfectAttendanceWindowDays);
    DateOnly monthlyStart = new DateOnly(today.Year, today.Month, 1).AddMonths(-MonthlyTrendWindowMonths);
    DateOnly attendanceStart = MinDateOnly(weeklyStart, perfectStart, monthlyStart);

    AttendanceByDateRangeSpec attendanceSpec = new(attendanceStart, today, request.Tenant);
    List<Attendance> attendance = await attendanceRepository.ListAsync(attendanceSpec, cancellationToken);

    DateTime leaveStart = DateTime.Today.AddMonths(-LeaveDistributionWindowMonths);
    LeavesByDateRangeSpec leaveSpec = new(leaveStart, DateTime.Today, request.Tenant);
    List<Leave> leaves = await leaveRepository.ListAsync(leaveSpec, cancellationToken);

    HomeDashboardDTO dashboard = new()
    {
      AttendanceSummary = BuildAttendanceSummary(attendance, today),
      WeeklyAttendanceTrend = BuildWeeklyAttendanceTrend(attendance, weeklyStart, today),
      LeaveDistribution = BuildLeaveDistribution(leaves),
      PerfectAttendanceLeaders = BuildPerfectAttendanceLeaders(attendance, perfectStart, today),
      MonthlyAbsenceTrend = BuildMonthlyAbsenceTrend(attendance, monthlyStart, today),
      GeneratedAtUtc = DateTime.UtcNow
    };

    return Result.Success(dashboard);
  }

  private static List<ChartSegmentDTO> BuildAttendanceSummary(IEnumerable<Attendance> attendance, DateOnly today)
  {
    int present = 0;
    int late = 0;
    int onLeave = 0;
    int absent = 0;

    foreach (Attendance record in attendance.Where(a => a.EntryDate.HasValue && a.EntryDate.Value == today))
    {
      if (IsRestDay(record))
      {
        continue;
      }

      if (IsOnLeave(record))
      {
        onLeave++;
        continue;
      }

      if (HasTimesheet(record))
      {
        if (IsLate(record))
        {
          late++;
        }
        else
        {
          present++;
        }

        continue;
      }

      if (IsAbsent(record))
      {
        absent++;
      }
    }

    return new()
    {
      new ChartSegmentDTO("Present", present),
      new ChartSegmentDTO("Late", late),
      new ChartSegmentDTO("On Leave", onLeave),
      new ChartSegmentDTO("Absent", absent)
    };
  }

  private static List<TrendPointDTO> BuildWeeklyAttendanceTrend(IEnumerable<Attendance> attendance, DateOnly start, DateOnly end)
  {
    Dictionary<DateOnly, int> counts = attendance
      .Where(record => record.EntryDate.HasValue)
      .Where(record => record.EntryDate!.Value >= start && record.EntryDate.Value <= end)
      .Where(record => !IsRestDay(record) && !IsOnLeave(record))
      .Where(record => HasTimesheet(record))
      .GroupBy(record => record.EntryDate!.Value)
      .ToDictionary(group => group.Key, group => group.Count());

    List<TrendPointDTO> result = new();
    DateOnly cursor = start;
    while (cursor <= end)
    {
      counts.TryGetValue(cursor, out int value);
      string label = cursor.ToString("MMM d", CultureInfo.InvariantCulture);
      result.Add(new TrendPointDTO(label, value));
      cursor = cursor.AddDays(1);
    }

    return result;
  }

  private static List<ChartSegmentDTO> BuildLeaveDistribution(IEnumerable<Leave> leaves)
  {
    Dictionary<string, decimal> totals = new(StringComparer.InvariantCultureIgnoreCase);

    foreach (Leave leave in leaves)
    {
      string label = ResolveLeaveLabel(leave);
      decimal duration = CalculateLeaveDuration(leave);

      if (totals.TryGetValue(label, out decimal existing))
      {
        totals[label] = existing + duration;
      }
      else
      {
        totals[label] = duration;
      }
    }

    return totals
      .OrderByDescending(pair => pair.Value)
      .Select(pair => new ChartSegmentDTO(pair.Key, pair.Value))
      .ToList();
  }

  private static List<PerfectAttendanceLeaderDTO> BuildPerfectAttendanceLeaders(IEnumerable<Attendance> attendance, DateOnly start, DateOnly end)
  {
    List<PerfectAttendanceLeaderDTO> leaders = new();

    var grouped = attendance
      .Where(record => record.EntryDate.HasValue && record.EntryDate.Value >= start && record.EntryDate.Value <= end)
      .Where(record => record.Employee is not null)
      .Where(record => !IsRestDay(record))
      .GroupBy(record => record.EmployeeId);

    foreach (IGrouping<Guid, Attendance> group in grouped)
    {
      Attendance first = group.First();
      string employeeName = first.Employee is null
        ? "Unknown"
        : string.Join(' ', new[] { first.Employee.FirstName, first.Employee.LastName }.Where(name => !string.IsNullOrWhiteSpace(name)));

      List<Attendance> tracked = group
        .Where(record => record.LeaveId is null)
        .ToList();

      if (tracked.Count == 0)
      {
        continue;
      }

      int perfectDays = tracked.Count(IsPerfectDay);
      int lateDays = tracked.Count(IsLate);
      int absences = tracked.Count(IsAbsent);
      int totalTracked = tracked.Count;

      decimal attendanceRate = totalTracked > 0
        ? Math.Round(perfectDays / (decimal)totalTracked, 4)
        : 0m;

      leaders.Add(new PerfectAttendanceLeaderDTO(
        group.Key,
        employeeName,
        totalTracked,
        perfectDays,
        lateDays,
        absences,
        attendanceRate));
    }

    List<PerfectAttendanceLeaderDTO> perfectLeaders = leaders
      .Where(leader => leader.TotalTrackedDays >= 5 && leader.PerfectDays == leader.TotalTrackedDays)
      .OrderByDescending(leader => leader.TotalTrackedDays)
      .ThenBy(leader => leader.EmployeeName)
      .Take(5)
      .ToList();

    if (perfectLeaders.Count < 5)
    {
      IEnumerable<PerfectAttendanceLeaderDTO> fallback = leaders
        .Where(leader => leader.TotalTrackedDays >= 5)
        .Except(perfectLeaders)
        .OrderByDescending(leader => leader.AttendanceRate)
        .ThenByDescending(leader => leader.PerfectDays)
        .ThenBy(leader => leader.EmployeeName)
        .Take(5 - perfectLeaders.Count);

      perfectLeaders.AddRange(fallback);
    }

    return perfectLeaders;
  }

  private static List<TrendPointDTO> BuildMonthlyAbsenceTrend(IEnumerable<Attendance> attendance, DateOnly start, DateOnly end)
  {
    Dictionary<(int Year, int Month), int> absences = new();

    foreach (Attendance record in attendance)
    {
      if (!record.EntryDate.HasValue)
      {
        continue;
      }

      DateOnly entryDate = record.EntryDate.Value;
      if (entryDate < start || entryDate > end)
      {
        continue;
      }

      if (IsAbsent(record))
      {
        (int Year, int Month) key = (entryDate.Year, entryDate.Month);
        absences[key] = absences.TryGetValue(key, out int existing) ? existing + 1 : 1;
      }
    }

    List<TrendPointDTO> result = new();
    DateOnly cursor = start;
    for (int i = 0; i < 12; i++)
    {
      (int Year, int Month) key = (cursor.Year, cursor.Month);
      absences.TryGetValue(key, out int value);
      string label = cursor.ToString("MMM yyyy", CultureInfo.InvariantCulture);
      result.Add(new TrendPointDTO(label, value));
      cursor = cursor.AddMonths(1);
    }

    return result;
  }

  private static DateOnly MinDateOnly(params DateOnly[] values)
  {
    if (values.Length == 0)
    {
      return DateOnly.FromDateTime(DateTime.Today);
    }

    DateOnly min = values[0];
    for (int i = 1; i < values.Length; i++)
    {
      if (values[i] < min)
      {
        min = values[i];
      }
    }

    return min;
  }

  private static bool IsRestDay(Attendance attendance)
  {
    string? shiftName = attendance.Shift?.Name;
    return attendance.ShiftId.HasValue && !string.IsNullOrWhiteSpace(shiftName) &&
      shiftName.Equals("R", StringComparison.InvariantCultureIgnoreCase);
  }

  private static bool HasTimesheet(Attendance attendance) => attendance.TimesheetId.HasValue || attendance.Timesheet is not null;

  private static bool IsOnLeave(Attendance attendance) => attendance.LeaveId.HasValue;

  private static bool IsLate(Attendance attendance)
  {
    return HasTimesheet(attendance) && (attendance.LateHrs ?? 0m) > 0m;
  }

  private static bool HasPenalty(Attendance attendance)
  {
    return (attendance.LateHrs ?? 0m) > 0m || (attendance.UnderHrs ?? 0m) > 0m || (attendance.OverbreakHrs ?? 0m) > 0m;
  }

  private static bool IsPerfectDay(Attendance attendance)
  {
    return !IsRestDay(attendance)
      && attendance.LeaveId is null
      && HasTimesheet(attendance)
      && !HasPenalty(attendance);
  }

  private static bool IsAbsent(Attendance attendance)
  {
    return !IsRestDay(attendance)
      && attendance.LeaveId is null
      && !HasTimesheet(attendance)
      && attendance.ShiftId.HasValue;
  }

  private static string ResolveLeaveLabel(Leave leave)
  {
    if (!string.IsNullOrWhiteSpace(leave.LeaveCredit?.LeaveDescription))
    {
      return leave.LeaveCredit.LeaveDescription;
    }

    if (!string.IsNullOrWhiteSpace(leave.LeaveCredit?.LeaveCode))
    {
      return leave.LeaveCredit.LeaveCode;
    }

    return "Other";
  }

  private static decimal CalculateLeaveDuration(Leave leave)
  {
    if (leave.IsHalfDay)
    {
      return 0.5m;
    }

    DateTime startDate = leave.StartDate.Date;
    DateTime endDate = leave.EndDate.Date;

    if (endDate < startDate)
    {
      return 0m;
    }

    return (decimal)(endDate - startDate).TotalDays + 1m;
  }
}
