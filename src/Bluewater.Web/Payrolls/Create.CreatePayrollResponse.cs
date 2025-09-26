namespace Bluewater.Web.Payrolls;

public class CreatePayrollResponse(Guid PayrollId)
{
  public Guid PayrollId { get; set; } = PayrollId;
}
