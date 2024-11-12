namespace Bluewater.UseCases.Payrolls;
public class PayrollSummaryDTO
{
    public DateOnly Date { get; set; }
    public int Count { get; set; }
    public decimal TotalServiceCharge { get; set; }
    public int TotalAbsences { get; set; }
    public decimal TotalLeaves { get; set; }
    public decimal TotalLates { get; set; }
    public decimal TotalUndertimes { get; set; }
    public decimal TotalOverbreak { get; set; }
    public decimal TotalTaxDeductions { get; set; }
    public decimal TotalNetAmount { get; set; }

}
