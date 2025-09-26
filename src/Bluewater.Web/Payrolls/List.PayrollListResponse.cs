using Bluewater.UseCases.Payrolls;

namespace Bluewater.Web.Payrolls;

public class PayrollListResponse
{
  public List<PayrollDTO> Payrolls { get; set; } = new();
}
