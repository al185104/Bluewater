using Bluewater.UseCases.Schedules;
using Bluewater.UseCases.Shifts;

namespace Bluewater.Web.Schedules;

public static class ScheduleMapper
{
  public static ScheduleRecord ToRecord(ScheduleDTO dto)
  {
    return new ScheduleRecord(
      dto.Id,
      dto.EmployeeId,
      dto.Name,
      dto.ShiftId,
      dto.ScheduleDate,
      dto.IsDefault,
      ToRecord(dto.Shift));
  }

  public static EmployeeScheduleRecord ToRecord(EmployeeScheduleDTO dto)
  {
    return new EmployeeScheduleRecord(
      dto.EmployeeId,
      dto.Barcode,
      dto.Name,
      dto.Section,
      dto.Charging,
      dto.Shifts.Select(shift => new ShiftInfoRecord(
        shift.ScheduleId,
        ToRecord(shift.Shift),
        shift.ScheduleDate,
        shift.IsDefault,
        shift.IsUpdated)).ToList());
  }

  private static ShiftDetailsRecord? ToRecord(ShiftDTO? shift)
  {
    if (shift is null) return null;

    return new ShiftDetailsRecord(
      shift.Id,
      shift.Name,
      shift.ShiftStartTime,
      shift.ShiftBreakTime,
      shift.ShiftBreakEndTime,
      shift.ShiftEndTime,
      shift.BreakHours);
  }
}
