using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Shifts;
using Bluewater.UserCases.Forms.Enum;
using FluentAssertions;

namespace Bluewater.UnitTests.UseCases.Attendances;

public class AttendanceSummaryCalculatorTests
{
  [Fact]
  public void CountApprovedLeaves_CountsOnlyApprovedLeaveRecords()
  {
    List<AttendanceDTO> attendances =
    [
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), 0, 0, 0, 0, 0, ApplicationStatusDTO.Approved),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 0, 0, 0, 0, 0, ApplicationStatusDTO.Pending),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddDays(2)), 0, 0, 0, 0, 0, ApplicationStatusDTO.Rejected),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null, DateOnly.FromDateTime(DateTime.Today.AddDays(3)), 8, 0, 0, 0, 0)
    ];

    int totalLeaves = AttendanceSummaryCalculator.CountApprovedLeaves(attendances);

    totalLeaves.Should().Be(1);
  }

  [Fact]
  public void CountAbsencesExcludingApprovedLeaves_DoesNotTreatApprovedLeaveAsAbsence()
  {
    ShiftDTO shift = new(Guid.NewGuid(), "D", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);

    List<AttendanceDTO> attendances =
    [
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), shift.Id, null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), 0, 0, 0, 0, 0, ApplicationStatusDTO.Approved, shift: shift),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), shift.Id, null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 0, 0, 0, 0, 0, ApplicationStatusDTO.Pending, shift: shift),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), shift.Id, Guid.NewGuid(), null, DateOnly.FromDateTime(DateTime.Today.AddDays(2)), 8, 0, 0, 0, 0, shift: shift)
    ];

    int totalAbsences = AttendanceSummaryCalculator.CountAbsencesExcludingApprovedLeaves(attendances);

    totalAbsences.Should().Be(1);
  }


  [Fact]
  public void CountAbsencesExcludingApprovedLeaves_DoesNotTreatHolidayAsAbsence()
  {
    ShiftDTO shift = new(Guid.NewGuid(), "D", new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(13, 0), new TimeOnly(17, 0), 1);
    DateOnly holidayDate = DateOnly.FromDateTime(DateTime.Today);

    List<AttendanceDTO> attendances =
    [
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), shift.Id, null, null, holidayDate, 0, 0, 0, 0, 0, shift: shift),
      new AttendanceDTO(Guid.NewGuid(), Guid.NewGuid(), shift.Id, null, null, holidayDate.AddDays(1), 0, 0, 0, 0, 0, shift: shift)
    ];

    int totalAbsences = AttendanceSummaryCalculator.CountAbsencesExcludingApprovedLeaves(attendances, [holidayDate]);

    totalAbsences.Should().Be(1);
  }

}