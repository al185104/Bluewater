using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Chargings;

public class CreateChargingRequest
{
  public const string Route = "/Chargings";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }
}
