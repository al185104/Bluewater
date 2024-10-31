using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.UndertimeAggregate;
public class Undertime(Guid empId, decimal inclusiveTime, string reason, DateOnly date) : EntityBase<Guid>, IAggregateRoot
{
    public decimal InclusiveTime { get; private set; } = inclusiveTime;
    public DateOnly Date { get; private set; } = date;
    public string Reason { get; private set; } = reason;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;
    // foreign key
    public Guid EmployeeId { get; private set; } = empId;
    public virtual Employee Employee { get; set; } = null!;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public Undertime() : this(Guid.Empty, 0, "", DateOnly.FromDateTime(DateTime.Now)) { }

    public void UpdateUndertime(Guid empId, decimal inclusiveTime, string reason, DateOnly date, ApplicationStatus status)
    {
        EmployeeId = empId;
        InclusiveTime = inclusiveTime;
        Reason = reason;
        Date = date;
        Status = status;

        UpdatedDate = DateTime.Now;
    }
}
