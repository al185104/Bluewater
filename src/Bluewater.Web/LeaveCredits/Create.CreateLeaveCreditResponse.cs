namespace Bluewater.Web.LeaveCredits;

public class CreateLeaveCreditResponse(Guid Id, string Code, string Description, decimal DefaultCredits, int SortOrder, bool IsLeaveWithPay, bool IsCanCarryOver)
{
  public Guid Id { get; set; } = Id;
  public string Code { get; set; } = Code;
  public string Description { get; set; } = Description;
  public decimal DefaultCredits { get; set; } = DefaultCredits;
  public int SortOrder { get; set; } = SortOrder;
  public bool IsLeaveWithPay { get; set; } = IsLeaveWithPay;
  public bool IsCanCarryOver { get; set; } = IsCanCarryOver;
}
