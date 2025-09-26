namespace Bluewater.Web.LeaveCredits;

public record LeaveCreditRecord(Guid Id, string Code, string Description, decimal DefaultCredits, int SortOrder, bool IsLeaveWithPay, bool IsCanCarryOver);
