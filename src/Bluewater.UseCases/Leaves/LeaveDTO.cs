
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Leaves;

public record LeaveDTO()
{
    public Guid Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsHalfDay { get; set; }
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;
    public Guid LeaveCreditId { get; set; }

    public LeaveDTO(Guid id, DateTime? startDate, DateTime? endDate, bool isHalfDay, ApplicationStatus status, Guid leaveCreditId) : this()
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        IsHalfDay = isHalfDay;
        Status = status;
        LeaveCreditId = leaveCreditId;
    }
}
