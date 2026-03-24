namespace Bluewater.Web.Deductions;

public class CreateDeductionResponse(DeductionRecord Deduction)
{
  public DeductionRecord Deduction { get; set; } = Deduction;
}
