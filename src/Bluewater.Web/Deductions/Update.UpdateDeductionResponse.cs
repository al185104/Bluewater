namespace Bluewater.Web.Deductions;

public class UpdateDeductionResponse(DeductionRecord Deduction)
{
  public DeductionRecord Deduction { get; set; } = Deduction;
}
