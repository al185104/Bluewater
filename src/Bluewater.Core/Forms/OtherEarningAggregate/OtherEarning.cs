using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.OtherEarningAggregate;

public class OtherEarning(Guid empId, OtherEarningType? type, decimal? totalAmount, bool isActive, DateOnly? date) : EntityBase<Guid>, IAggregateRoot
{    
    public OtherEarningType? EarningType { get; set; } = type;
    public decimal? TotalAmount { get; set; } = totalAmount;
    public bool IsActive { get; set; } = isActive;
    public DateOnly? Date { get; set; } = date;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;

    public Guid? EmployeeId { get; set; } = empId;
    public virtual Employee? Employee { get; set; }

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public OtherEarning() : this(Guid.Empty, OtherEarningType.NotSet, 0, false, DateOnly.FromDateTime(DateTime.Now)) { }

    public void UpdateOtherEarning(Guid empId, OtherEarningType type, decimal totalAmount, bool isActive, DateOnly date, ApplicationStatus status)
    {
        EmployeeId = empId;
        EarningType = type;
        TotalAmount = totalAmount;
        IsActive = isActive;
        Date = date;
        Status = status;

        UpdatedDate = DateTime.Now;
    }
}