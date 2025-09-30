using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.Core.ScheduleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data;

public static class ScheduleDataSeeder
{
  private static readonly IReadOnlyList<ScheduleSeedInfo> SchedulesToSeed = new List<ScheduleSeedInfo>
  {
    new("jdoe", "A1", new DateOnly(2025, 1, 6), true),
    new("mlopez", "A2", new DateOnly(2025, 1, 6), false),
    new("kchan", "A3", new DateOnly(2025, 1, 6), false),
    new("areyes", "A4", new DateOnly(2025, 1, 6), false),
    new("jdoe", "A5", new DateOnly(2025, 1, 7), false),
    new("mlopez", "A6", new DateOnly(2025, 1, 7), false)
  };

  public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(context);

    if (await context.Schedules.AnyAsync(cancellationToken))
    {
      return;
    }

    var usernames = SchedulesToSeed
      .Select(schedule => schedule.EmployeeUsername)
      .Distinct()
      .ToArray();

    var employees = await context.Employees
      .Include(e => e.User)
      .Where(e => e.User != null && usernames.Contains(e.User.Username))
      .ToDictionaryAsync(e => e.User!.Username, cancellationToken);

    if (employees.Count != usernames.Length)
    {
      throw new InvalidOperationException("One or more seed employees are missing. Ensure employee seeding runs first.");
    }

    var shiftNames = SchedulesToSeed
      .Select(schedule => schedule.ShiftName)
      .Distinct()
      .ToArray();

    var shifts = await context.Shifts
      .Where(shift => shiftNames.Contains(shift.Name))
      .ToDictionaryAsync(shift => shift.Name, cancellationToken);

    if (shifts.Count != shiftNames.Length)
    {
      throw new InvalidOperationException("One or more seed shifts are missing. Ensure shift seeding runs first.");
    }

    foreach (var schedule in SchedulesToSeed)
    {
      var employee = employees[schedule.EmployeeUsername];
      var shift = shifts[schedule.ShiftName];

      var exists = await context.Schedules.AnyAsync(
        s => s.EmployeeId == employee.Id && s.ShiftId == shift.Id && s.ScheduleDate == schedule.ScheduleDate,
        cancellationToken);

      if (exists)
      {
        continue;
      }

      context.Schedules.Add(new Schedule(employee.Id, shift.Id, schedule.ScheduleDate, schedule.IsDefault));
    }

    await context.SaveChangesAsync(cancellationToken);
  }

  private sealed record ScheduleSeedInfo(string EmployeeUsername, string ShiftName, DateOnly ScheduleDate, bool IsDefault);
}
