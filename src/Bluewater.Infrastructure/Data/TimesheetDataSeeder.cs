using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.Core.TimesheetAggregate;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data;

public static class TimesheetDataSeeder
{
  private static readonly IReadOnlyList<string> EmployeeUsernames = new[]
  {
    "jdoe",
    "mlopez",
    "kchan",
    "areyes"
  };

  private static readonly DateOnly StartDate = new(2024, 8, 1);
  private static readonly DateOnly EndDate = new(2024, 10, 31);

  public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(context);

    if (await context.Timesheets.AnyAsync(cancellationToken))
    {
      return;
    }

    var employees = await context.Employees
      .Include(e => e.User)
      .Where(e => e.User != null && EmployeeUsernames.Contains(e.User.Username))
      .ToDictionaryAsync(e => e.User!.Username, cancellationToken);

    if (employees.Count != EmployeeUsernames.Count)
    {
      throw new InvalidOperationException("One or more seed employees are missing. Ensure employee seeding runs first.");
    }

    var timesheets = new List<Timesheet>();

    foreach (var username in EmployeeUsernames)
    {
      var employee = employees[username];

      foreach (var date in EnumerateDates(StartDate, EndDate))
      {
        var schedule = GetDailySchedule(username, date);

        var timesheet = new Timesheet(
          employee.Id,
          ToDateTime(date, schedule.TimeIn1),
          ToDateTime(date, schedule.TimeOut1),
          ToDateTime(date, schedule.TimeIn2),
          ToDateTime(date, schedule.TimeOut2),
          date);

        timesheets.Add(timesheet);
      }
    }

    context.Timesheets.AddRange(timesheets);
    await context.SaveChangesAsync(cancellationToken);
  }

  private static IEnumerable<DateOnly> EnumerateDates(DateOnly start, DateOnly end)
  {
    for (var date = start; date <= end; date = date.AddDays(1))
    {
      yield return date;
    }
  }

  private static DailySchedule GetDailySchedule(string username, DateOnly date)
  {
    if (date.DayOfWeek == DayOfWeek.Sunday)
    {
      return DailySchedule.RestDay;
    }

    var isSaturday = date.DayOfWeek == DayOfWeek.Saturday;

    return username switch
    {
      "jdoe" => isSaturday
        ? new DailySchedule(new TimeOnly(9, 0), new TimeOnly(12, 0), null, null)
        : new DailySchedule(new TimeOnly(8, 30), new TimeOnly(12, 30), new TimeOnly(13, 30), new TimeOnly(17, 30)),
      "mlopez" => isSaturday
        ? new DailySchedule(new TimeOnly(10, 0), new TimeOnly(14, 0), null, null)
        : new DailySchedule(new TimeOnly(9, 0), new TimeOnly(12, 30), new TimeOnly(13, 30), new TimeOnly(18, 0)),
      "kchan" => isSaturday
        ? new DailySchedule(new TimeOnly(15, 0), new TimeOnly(20, 0), null, null)
        : new DailySchedule(new TimeOnly(14, 0), new TimeOnly(18, 0), new TimeOnly(19, 0), new TimeOnly(23, 0)),
      "areyes" => isSaturday
        ? new DailySchedule(new TimeOnly(8, 0), new TimeOnly(12, 0), null, null)
        : new DailySchedule(new TimeOnly(7, 30), new TimeOnly(11, 30), new TimeOnly(12, 30), new TimeOnly(16, 30)),
      _ => throw new InvalidOperationException($"No schedule configured for username '{username}'.")
    };
  }

  private static DateTime? ToDateTime(DateOnly date, TimeOnly? time)
  {
    return time.HasValue ? date.ToDateTime(time.Value) : null;
  }

  private sealed record DailySchedule(TimeOnly? TimeIn1, TimeOnly? TimeOut1, TimeOnly? TimeIn2, TimeOnly? TimeOut2)
  {
    public static DailySchedule RestDay { get; } = new(null, null, null, null);
  }
}
