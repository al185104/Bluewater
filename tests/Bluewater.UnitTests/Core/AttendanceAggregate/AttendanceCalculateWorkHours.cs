using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;
using FluentAssertions;
using Xunit;

namespace Bluewater.UnitTests.Core.AttendanceAggregate;

public class AttendanceCalculateWorkHours
{
  [Fact]
  public void CountsOnlyHoursInsideScheduledWindowForSingleSession()
  {
    DateTime shiftDate = new(2024, 1, 15, 0, 0, 0);
    Shift shift = new("DAY", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);
    Timesheet timesheet = new(Guid.NewGuid(),
      shiftDate.AddHours(7),
      shiftDate.AddHours(18),
      null,
      null,
      DateOnly.FromDateTime(shiftDate));

    Attendance attendance = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateOnly.FromDateTime(shiftDate), null, null, null, null, null)
    {
      Shift = shift,
      Timesheet = timesheet
    };

    var result = attendance.CalculateWorkHours();

    result.totalWorkHours.Should().Be(9m);
  }

  [Fact]
  public void SumsOnlySessionOverlapsWithinSchedule()
  {
    DateTime shiftDate = new(2024, 1, 15, 0, 0, 0);
    Shift shift = new("DAY", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);
    Timesheet timesheet = new(Guid.NewGuid(),
      shiftDate.AddHours(6),
      shiftDate.AddHours(11),
      shiftDate.AddHours(14),
      shiftDate.AddHours(20),
      DateOnly.FromDateTime(shiftDate));

    Attendance attendance = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateOnly.FromDateTime(shiftDate), null, null, null, null, null)
    {
      Shift = shift,
      Timesheet = timesheet
    };

    var result = attendance.CalculateWorkHours();

    result.totalWorkHours.Should().Be(6m);
  }
}
