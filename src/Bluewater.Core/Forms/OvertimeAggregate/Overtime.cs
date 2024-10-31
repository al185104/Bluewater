using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.OvertimeAggregate;
public class Overtime(Guid empId, DateTime startDate, DateTime endDate, int approvedHours, string remarks) : EntityBase<Guid>, IAggregateRoot
{
    public DateTime StartDate { get; private set; } = startDate;
    public DateTime EndDate { get; private set; } = endDate;
    public int ApprovedHours { get; private set; } = approvedHours;
    public string Remarks { get; private set; } = remarks;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;
    // foreign key
    public Guid EmployeeId { get; private set; } = empId;
    public virtual Employee Employee { get; set; } = null!;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public Overtime() : this(Guid.Empty, DateTime.Now, DateTime.Now, 0, "") { }

    public void UpdateOvertime(Guid empId, DateTime startDate, DateTime endDate, int approvedHours, string remarks, ApplicationStatus status)
    {
        EmployeeId = empId;
        StartDate = startDate;
        EndDate = endDate;
        ApprovedHours = approvedHours;
        Remarks = remarks;
        Status = status;

        UpdatedDate = DateTime.Now;
    }    
}
