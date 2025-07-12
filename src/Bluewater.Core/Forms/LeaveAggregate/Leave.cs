using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Forms.LeaveAggregate;
public class Leave (Guid empId, Guid leaveCreditId, DateTime startDate, DateTime endDate, bool isHalfDay) : EntityBase<Guid>, IAggregateRoot
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
    public bool IsHalfDay { get; set; } = isHalfDay;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;
    public Guid? EmployeeId { get; set; } = empId;
    public Guid LeaveCreditId { get; set; } = leaveCreditId;
    public virtual Employee? Employee { get; set; }
    public virtual LeaveCredit? LeaveCredit { get; set; }

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public Leave() : this(Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, false) { }

    public void UpdateLeave(Guid empId, Guid leaveCreditId, DateTime startDate, DateTime endDate, bool isHalfDay, ApplicationStatus status)
    {
        EmployeeId = empId;
        LeaveCreditId = leaveCreditId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        IsHalfDay = isHalfDay;

        UpdatedDate = DateTime.Now;
    }    
}
