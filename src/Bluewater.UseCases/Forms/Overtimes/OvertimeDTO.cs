
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes;
public record OvertimeDTO() {
    public Guid Id { get; init; }
    public Guid EmpId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ApprovedHours { get; set; }
    public string? Remarks { get; set; }
    public ApplicationStatusDTO Status { get; set; } = ApplicationStatusDTO.NotSet;

    public OvertimeDTO(Guid id, Guid empId, DateTime? startDate, DateTime? endDate, int? approvedHours, string? remarks, ApplicationStatusDTO status) : this()
    {
        Id = id;
        EmpId = empId;
        StartDate = startDate;
        EndDate = endDate;
        ApprovedHours = approvedHours;
        Remarks = remarks;
        Status = status;
    }
}
