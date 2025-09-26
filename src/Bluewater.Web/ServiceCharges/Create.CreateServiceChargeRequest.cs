using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.ServiceCharges;

public class CreateServiceChargeRequest
{
  public const string Route = "/ServiceCharges";

  [Required]
  public string Username { get; set; } = string.Empty;

  [Range(0, double.MaxValue)]
  public decimal Amount { get; set; }

  [Required]
  public DateOnly Date { get; set; }
}
