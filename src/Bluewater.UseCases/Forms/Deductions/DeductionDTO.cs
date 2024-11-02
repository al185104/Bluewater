using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions;
public record DeductionDTO()
{
    public Guid Id { get; init; }
    public Guid? EmpId { get; set; }
    public string? Name { get; set; }
    public DeductionsTypeDTO? Type { get; set; } = DeductionsTypeDTO.NotSet;
    public decimal? TotalAmount { get; set; }
    public decimal? MonthlyAmortization { get; set; }
    public decimal? RemainingBalance { get; set; }
    public int? NoOfMonths { get; set; }
    public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly? EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string? Remarks { get; set; }
    public ApplicationStatusDTO? Status { get; set; }

    public DeductionDTO(Guid Id, Guid? EmpId, string? Name, DeductionsTypeDTO? Type, decimal? TotalAmount, decimal? MonthlyAmortization, decimal? RemainingBalance, int? NoOfMonths, DateOnly? StartDate, DateOnly? EndDate, string? Remarks, ApplicationStatusDTO? Status) : this()
    {
        this.Id = Id;
        this.EmpId = EmpId;
        this.Name = Name;
        this.Type = Type;
        this.TotalAmount = TotalAmount;
        this.MonthlyAmortization = MonthlyAmortization;
        this.RemainingBalance = RemainingBalance;
        this.NoOfMonths = NoOfMonths;
        this.StartDate = StartDate;
        this.EndDate = EndDate;
        this.Remarks = Remarks;
        this.Status = Status;
    }
}