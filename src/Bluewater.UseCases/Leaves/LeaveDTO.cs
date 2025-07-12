using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves;

public record LeaveDTO()
{
    public Guid Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsHalfDay { get; set; }
    public ApplicationStatusDTO Status { get; private set; } = ApplicationStatusDTO.NotSet;
    public Guid? EmployeeId { get; set; }
    public Guid LeaveCreditId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveCreditName { get; set; } = string.Empty;

    public LeaveDTO(Guid id, DateTime? startDate, DateTime? endDate, bool isHalfDay, ApplicationStatusDTO status, Guid employeeId, Guid leaveCreditId) : this()
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        IsHalfDay = isHalfDay;
        Status = status;
        EmployeeId = employeeId;
        LeaveCreditId = leaveCreditId;
    }

    public LeaveDTO(Guid id, DateTime? startDate, DateTime? endDate, bool isHalfDay, ApplicationStatusDTO status, Guid employeeId, Guid leaveCreditId, string employeeName, string leaveCreditName) : this()
    {
      Id = id;
      StartDate = startDate;
      EndDate = endDate;
      IsHalfDay = isHalfDay;
      Status = status;
      EmployeeId = employeeId;
      LeaveCreditId = leaveCreditId;
      EmployeeName = employeeName;
      LeaveCreditName = leaveCreditName;
    } 
}
