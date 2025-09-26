using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Payrolls;

public class PayrollGroupedListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }
}
