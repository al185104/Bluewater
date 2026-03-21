using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Shifts;
using FluentAssertions;

namespace Bluewater.UnitTests.UseCases.Attendances;

public class ApprovedLeaveWorkHoursCalculatorTests
{
  [Fact]
  public void CreateSyntheticTimesheet_BuildsFullShiftSessionsForDaySchedule()
  {
    DateOnly entryDate = new(2025, 2, 10);
    ShiftDTO shift = new(Guid.NewGuid(), "DAY", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);

    var result = ApprovedLeaveWorkHoursCalculator.CreateSyntheticTimesheet(Guid.NewGuid(), entryDate, shift);

    result.Should().NotBeNull();
    result!.TimeIn1.Should().Be(entryDate.ToDateTime(new TimeOnly(8, 0)));
    result.TimeOut1.Should().Be(entryDate.ToDateTime(new TimeOnly(12, 0)));
    result.TimeIn2.Should().Be(entryDate.ToDateTime(new TimeOnly(13, 0)));
    result.TimeOut2.Should().Be(entryDate.ToDateTime(new TimeOnly(17, 0)));
  }

  [Fact]
  public void CreateSyntheticTimesheet_AdjustsOvernightShiftSegmentsToNextDay()
  {
    DateOnly entryDate = new(2025, 2, 10);
    ShiftDTO shift = new(Guid.NewGuid(), "NIGHT", new TimeOnly(22, 0), new TimeOnly(2, 0), new TimeOnly(3, 0), new TimeOnly(7, 0), 1);

    var result = ApprovedLeaveWorkHoursCalculator.CreateSyntheticTimesheet(Guid.NewGuid(), entryDate, shift);

    result.Should().NotBeNull();
    result!.TimeIn1.Should().Be(entryDate.ToDateTime(new TimeOnly(22, 0)));
    result.TimeOut1.Should().Be(entryDate.ToDateTime(new TimeOnly(2, 0)).AddDays(1));
    result.TimeIn2.Should().Be(entryDate.ToDateTime(new TimeOnly(3, 0)).AddDays(1));
    result.TimeOut2.Should().Be(entryDate.ToDateTime(new TimeOnly(7, 0)).AddDays(1));
  }
}
