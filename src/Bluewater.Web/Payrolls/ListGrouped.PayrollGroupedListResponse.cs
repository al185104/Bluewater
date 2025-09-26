using Bluewater.UseCases.Payrolls;

namespace Bluewater.Web.Payrolls;

public class PayrollGroupedListResponse
{
  public List<PayrollSummaryDTO> Payrolls { get; set; } = new();
}
