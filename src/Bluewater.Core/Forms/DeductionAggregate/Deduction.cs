using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.DeductionAggregate;
public class Deduction (Guid empId, DeductionsType type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks) : EntityBase<Guid>, IAggregateRoot
{
    public DeductionsType DeductionType { get; set; } = type;
    public decimal TotalAmount { get; set; } = totalAmount;
    public decimal MonthlyAmortization { get; set; } = monthlyAmortization;
    public decimal RemainingBalance { get; set; } = remainingBalance;
    public int NoOfMonths { get; set; } = noOfMonths;
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly EndDate { get; set; } = endDate;
    public string Remarks { get; set; } = remarks;
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.NotSet;

    // foreign key
    public Guid? EmployeeId { get; set; } = empId;
    public virtual Employee? Employee { get; set; } = null!;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public Deduction() : this(Guid.Empty, DeductionsType.NotSet, 0, 0, 0, 0, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now), "") { }

    public void UpdateDeduction(Guid empId, DeductionsType type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks, ApplicationStatus status)
    {
        EmployeeId = empId;
        DeductionType = type;
        TotalAmount = totalAmount;
        MonthlyAmortization = monthlyAmortization;
        RemainingBalance = remainingBalance;
        NoOfMonths = noOfMonths;
        StartDate = startDate;
        EndDate = endDate;
        Remarks = remarks;
        Status = status;

        UpdatedDate = DateTime.Now;
    }
}
