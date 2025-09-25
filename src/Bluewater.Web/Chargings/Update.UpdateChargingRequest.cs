using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Chargings;

public class UpdateChargingRequest
{
  public const string Route = "/Chargings";

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }

  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }
}
