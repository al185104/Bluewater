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

    result.totalWorkHours.Should().Be(8m);
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

  [Fact]
  public void AppliesPenaltyThresholdsAsWholeHourDeductions()
  {
    DateTime shiftDate = new(2024, 1, 15, 0, 0, 0);
    Shift shift = new("DAY", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);
    Timesheet timesheet = new(Guid.NewGuid(),
      shiftDate.AddHours(8).AddMinutes(20),   // 20 mins late
      shiftDate.AddHours(12),
      shiftDate.AddHours(14).AddMinutes(20),  // 80-min break (20 mins excess)
      shiftDate.AddHours(16).AddMinutes(40),  // 20 mins undertime
      DateOnly.FromDateTime(shiftDate));

    Attendance attendance = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateOnly.FromDateTime(shiftDate), null, null, null, null, null)
    {
      Shift = shift,
      Timesheet = timesheet
    };

    var result = attendance.CalculateWorkHours();

    result.totalLateHours.Should().Be(1m);
    result.totalOverbreakHours.Should().Be(1m);
    result.totalUndertimeHours.Should().Be(1m);
  }

  [Fact]
  public void CalculatesNightShiftHoursUsingConfigurableWindow()
  {
    DateTime shiftDate = new(2024, 1, 15, 0, 0, 0);
    Shift shift = new("NIGHT", new TimeOnly(20, 0), new TimeOnly(0, 0), new TimeOnly(1, 0), new TimeOnly(4, 0), 1);
    Timesheet timesheet = new(Guid.NewGuid(),
      shiftDate.AddHours(20),
      shiftDate.AddDays(1).AddHours(0),
      shiftDate.AddDays(1).AddHours(1),
      shiftDate.AddDays(1).AddHours(4),
      DateOnly.FromDateTime(shiftDate));

    Attendance attendance = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateOnly.FromDateTime(shiftDate), null, null, null, null, null)
    {
      Shift = shift,
      Timesheet = timesheet
    };

    var result = attendance.CalculateWorkHours(
      enableNightShiftComputation: true,
      nightShiftStartTime: new TimeOnly(21, 0),
      nightShiftEndTime: new TimeOnly(3, 0));

    result.totalNightShiftHours.Should().Be(5m);
  }
}
