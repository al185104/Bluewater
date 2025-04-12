
namespace Bluewater.UseCases.LeaveCredits;

public record LeaveCreditDTO()
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal DefaultCredits { get; set; }
    public bool IsLeaveWithPay { get; set; }
    public bool IsCanCarryOver { get; set; }
    public int SortOrder { get; set; }

    public LeaveCreditDTO(Guid id, string leaveCode, string leaveDescription, decimal defaultCredits, int sortOrder, bool isLeaveWithPay, bool isCanCarryOver) : this()
      {
          Id = id;
          Code = leaveCode;
          Description = leaveDescription;
          DefaultCredits = defaultCredits;
          SortOrder = sortOrder;
          IsLeaveWithPay = isLeaveWithPay;
          IsCanCarryOver = isCanCarryOver;
    }
}
