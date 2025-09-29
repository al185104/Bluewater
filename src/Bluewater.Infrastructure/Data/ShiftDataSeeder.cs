using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.Core.ShiftAggregate;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data;

public static class ShiftDataSeeder
{
  private static readonly IReadOnlyList<ShiftSeedInfo> ShiftsToSeed = new List<ShiftSeedInfo>
  {
    new("A1", new TimeOnly(0, 0), new TimeOnly(3, 0), new TimeOnly(4, 0), new TimeOnly(8, 0), 1m),
    new("A2", new TimeOnly(1, 0), new TimeOnly(4, 0), new TimeOnly(5, 0), new TimeOnly(9, 0), 1m),
    new("A3", new TimeOnly(2, 0), new TimeOnly(5, 0), new TimeOnly(6, 0), new TimeOnly(10, 0), 1m),
    new("A4", new TimeOnly(3, 0), new TimeOnly(6, 0), new TimeOnly(7, 0), new TimeOnly(11, 0), 1m),
    new("A5", new TimeOnly(4, 0), new TimeOnly(7, 0), new TimeOnly(8, 0), new TimeOnly(12, 0), 1m),
    new("A6", new TimeOnly(5, 0), new TimeOnly(8, 0), new TimeOnly(9, 0), new TimeOnly(13, 0), 1m),
    new("A7", new TimeOnly(6, 0), new TimeOnly(9, 0), new TimeOnly(10, 0), new TimeOnly(14, 0), 1m),
    new("A8", new TimeOnly(7, 0), new TimeOnly(10, 0), new TimeOnly(11, 0), new TimeOnly(15, 0), 1m),
    new("A9", new TimeOnly(8, 0), new TimeOnly(11, 0), new TimeOnly(12, 0), new TimeOnly(16, 0), 1m),
    new("A10", new TimeOnly(9, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1m),
    new("A11", new TimeOnly(10, 0), new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(18, 0), 1m),
    new("A12", new TimeOnly(11, 0), new TimeOnly(14, 0), new TimeOnly(15, 0), new TimeOnly(19, 0), 1m),
    new("A13", new TimeOnly(12, 0), new TimeOnly(15, 0), new TimeOnly(16, 0), new TimeOnly(20, 0), 1m),
    new("A14", new TimeOnly(13, 0), new TimeOnly(16, 0), new TimeOnly(17, 0), new TimeOnly(21, 0), 1m),
    new("A15", new TimeOnly(14, 0), new TimeOnly(17, 0), new TimeOnly(18, 0), new TimeOnly(22, 0), 1m),
    new("A16", new TimeOnly(15, 0), new TimeOnly(18, 0), new TimeOnly(19, 0), new TimeOnly(23, 0), 1m),
    new("A17", new TimeOnly(16, 0), new TimeOnly(19, 0), new TimeOnly(20, 0), new TimeOnly(0, 0), 1m),
    new("A18", new TimeOnly(17, 0), new TimeOnly(20, 0), new TimeOnly(21, 0), new TimeOnly(1, 0), 1m),
    new("A19", new TimeOnly(18, 0), new TimeOnly(21, 0), new TimeOnly(22, 0), new TimeOnly(2, 0), 1m),
    new("A20", new TimeOnly(19, 0), new TimeOnly(22, 0), new TimeOnly(23, 0), new TimeOnly(3, 0), 1m),
    new("R", null, null, null, null, 0m)
  };

  public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(context);

    var hasChanges = false;

    foreach (var shift in ShiftsToSeed)
    {
      var exists = await context.Shifts.AnyAsync(s => s.Name == shift.Name, cancellationToken);
      if (exists)
      {
        continue;
      }

      context.Shifts.Add(new Shift(shift.Name, shift.Start, shift.BreakStart, shift.BreakEnd, shift.End, shift.BreakHours));
      hasChanges = true;
    }

    if (hasChanges)
    {
      await context.SaveChangesAsync(cancellationToken);
    }
  }

  private sealed record ShiftSeedInfo(
    string Name,
    TimeOnly? Start,
    TimeOnly? BreakStart,
    TimeOnly? BreakEnd,
    TimeOnly? End,
    decimal BreakHours);
}
