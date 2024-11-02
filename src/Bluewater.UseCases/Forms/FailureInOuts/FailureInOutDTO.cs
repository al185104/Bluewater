using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts;
public record FailureInOutDTO()
{
    public Guid Id { get; init; }
    public Guid? EmpId { get; set; }
    public DateTime? Date { get; set; } = DateTime.Now;
    public string? Remarks { get; set; }
    public FailureInOutReasonDTO? Reason { get; set; } = FailureInOutReasonDTO.NotSet;
    public ApplicationStatusDTO? Status { get; set; }

    public FailureInOutDTO(Guid Id, Guid? EmpId, DateTime? Date, string? Remarks, FailureInOutReasonDTO? Reason, ApplicationStatusDTO? Status) : this()
    {
        this.Id = Id;
        this.EmpId = EmpId;
        this.Date = Date;
        this.Remarks = Remarks;
        this.Reason = Reason;
        this.Status = Status;
    }
}