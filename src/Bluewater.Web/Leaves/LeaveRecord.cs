using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.Web.Leaves;

public record LeaveRecord(
  Guid Id,
  DateTime? StartDate,
  DateTime? EndDate,
  bool IsHalfDay,
  ApplicationStatusDTO Status,
  Guid? EmployeeId,
  Guid LeaveCreditId,
  string EmployeeName,
  string LeaveCreditName);
