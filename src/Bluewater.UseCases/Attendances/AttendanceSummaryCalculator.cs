using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Attendances;

public static class AttendanceSummaryCalculator
{
  public static bool HasApprovedLeave(AttendanceDTO attendance) =>
    attendance.LeaveId.HasValue
    && attendance.LeaveId != Guid.Empty
    && attendance.LeaveStatus == ApplicationStatusDTO.Approved;

  public static int CountApprovedLeaves(IEnumerable<AttendanceDTO> attendances) =>
    attendances.Count(HasApprovedLeave);

  public static int CountAbsencesExcludingApprovedLeaves(IEnumerable<AttendanceDTO> attendances) =>
    attendances.Count(attendance =>
      attendance.ShiftId != null
      && attendance.Shift != null
      && !attendance.Shift.Name.Equals("R", StringComparison.InvariantCultureIgnoreCase)
      && !HasApprovedLeave(attendance)
      && (attendance.WorkHrs ?? 0) <= 0);
}
