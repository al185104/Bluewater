using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.EmployeeTypes;

public class UpdateEmployeeTypeRequest
{
  public const string Route = "/EmployeeTypes";

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }

  [Required]
  public string? Value { get; set; }

  public bool IsActive { get; set; }
}
