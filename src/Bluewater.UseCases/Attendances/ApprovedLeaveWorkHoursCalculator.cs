using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;

namespace Bluewater.UseCases.Attendances;

public static class ApprovedLeaveWorkHoursCalculator
{
  public static TimesheetDTO? CreateSyntheticTimesheet(Guid employeeId, DateOnly entryDate, ShiftDTO? shift)
  {
    if (shift?.ShiftStartTime is null || shift.ShiftEndTime is null)
    {
      return null;
    }

    DateTime timeIn1 = entryDate.ToDateTime(shift.ShiftStartTime.Value);
    DateTime timeOut1;
    DateTime? timeIn2 = null;
    DateTime? timeOut2 = null;

    if (shift.ShiftBreakTime.HasValue && shift.ShiftBreakEndTime.HasValue)
    {
      timeOut1 = entryDate.ToDateTime(shift.ShiftBreakTime.Value);
      timeIn2 = entryDate.ToDateTime(shift.ShiftBreakEndTime.Value);
      timeOut2 = entryDate.ToDateTime(shift.ShiftEndTime.Value);

      if (timeOut1 <= timeIn1)
      {
        timeOut1 = timeOut1.AddDays(1);
      }

      if (timeIn2 <= timeOut1)
      {
        timeIn2 = timeIn2.Value.AddDays(1);
      }

      if (timeOut2 <= timeIn2)
      {
        timeOut2 = timeOut2.Value.AddDays(1);
      }
    }
    else
    {
      timeOut1 = entryDate.ToDateTime(shift.ShiftEndTime.Value);
      if (timeOut1 <= timeIn1)
      {
        timeOut1 = timeOut1.AddDays(1);
      }
    }

    return new TimesheetDTO(
      Guid.Empty,
      employeeId,
      timeIn1,
      timeOut1,
      timeIn2,
      timeOut2,
      entryDate,
      isEdited: false,
      shiftId: shift.Id,
      shiftName: shift.Name);
  }
}
