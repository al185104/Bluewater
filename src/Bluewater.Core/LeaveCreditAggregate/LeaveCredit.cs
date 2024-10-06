using Ardalis.SharedKernel;

namespace Bluewater.Core.LeaveCreditAggregate;
public class LeaveCredit(string leaveCode, string leaveDescription, decimal defaultCredits, bool isLeaveWithPay, bool isCanCarryOver, int sortOrder) : EntityBase<Guid>, IAggregateRoot
{
  public string LeaveCode { get; private set; } = leaveCode;
  public string LeaveDescription { get; private set; } = leaveDescription;
  public decimal DefaultCredits { get; private set; } = defaultCredits;
  public bool IsLeaveWithPay { get; private set; } = isLeaveWithPay;
  public bool IsCanCarryOver { get; private set; } = isCanCarryOver;
  public int SortOrder { get; private set; } = sortOrder;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  public LeaveCredit() : this(string.Empty, string.Empty, 0, false, false, 0) { }
}
