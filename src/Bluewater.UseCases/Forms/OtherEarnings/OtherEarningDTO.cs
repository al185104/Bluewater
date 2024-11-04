
using Bluewater.UserCases.Forms.Enum;
namespace Bluewater.UseCases.Forms.OtherEarnings;
public record OtherEarningDTO()
{
    public Guid Id { get; init; }
    public Guid? EmpId { get; set; }
    public string? Name { get; set; }
    public OtherEarningTypeDTO? EarningType { get; set; } = OtherEarningTypeDTO.NotSet;
    public decimal? TotalAmount { get; set; }
    public bool IsActive { get; set; }
    public DateOnly? Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public ApplicationStatusDTO? Status { get; set; } = ApplicationStatusDTO.NotSet;

    public OtherEarningDTO(Guid id, Guid? empId, string? name, OtherEarningTypeDTO? type, decimal? totalAmount, bool isActive, DateOnly? date, ApplicationStatusDTO? status) : this()
    {
        Id = id;
        EmpId = empId;
        Name = name;
        EarningType = type;
        TotalAmount = totalAmount;
        IsActive = isActive;
        Date = date;
        Status = status;
    }
}