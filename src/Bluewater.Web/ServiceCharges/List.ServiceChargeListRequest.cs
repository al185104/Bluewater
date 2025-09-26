using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.ServiceCharges;

public class ServiceChargeListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  [Required]
  public DateOnly Date { get; set; }
}
