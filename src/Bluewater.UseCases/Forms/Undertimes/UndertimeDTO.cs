using Bluewater.UserCases.Forms.Enum;
namespace Bluewater.UseCases.Forms.Undertimes;
public record UndertimeDTO()
{
    public Guid Id { get; init; }
    public Guid? EmpId { get; set; }
    public string? Name { get; set; }
    public decimal? InclusiveTime { get; set; }
    public string? Reason { get; set; }
    public DateOnly? Date { get; set; }
    public ApplicationStatusDTO Status { get; set; } = ApplicationStatusDTO.NotSet;

    public UndertimeDTO(Guid id, Guid? empId, string? name, decimal? inclusiveTime, string? reason, DateOnly? date, ApplicationStatusDTO status) : this()
    {
        Id = id;
        EmpId = empId;
        Name = name;
        InclusiveTime = inclusiveTime;
        Reason = reason;
        Date = date;
        Status = status;
    }
}