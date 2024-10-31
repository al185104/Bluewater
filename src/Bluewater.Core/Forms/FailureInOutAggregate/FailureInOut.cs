using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.FailureInOutAggregate;
public class FailureInOut(Guid empId, DateTime date, string remarks, FailureInOutReason reason) : EntityBase<Guid>, IAggregateRoot
{
    public DateTime Date { get; set; } = date;
    public string Remarks { get; set; } = remarks;
    public FailureInOutReason Reason { get; set; } = reason;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;

    // foreign key
    public Guid? EmployeeId { get; set; } = empId;
    public virtual Employee? Employee { get; set; } = null!;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public FailureInOut() : this(Guid.Empty, DateTime.Now, "", FailureInOutReason.NotSet) { }

    public void UpdateFailureInOut(Guid empId, DateTime date, string remarks, ApplicationStatus status)
    {
        EmployeeId = empId;
        Date = date;
        Remarks = remarks;
        Status = status;

        UpdatedDate = DateTime.Now;
    }
}