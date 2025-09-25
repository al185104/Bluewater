using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.EmployeeTypes;

public class CreateEmployeeTypeRequest
{
  public const string Route = "/EmployeeTypes";

  [Required]
  public string? Name { get; set; }

  [Required]
  public string? Value { get; set; }

  public bool IsActive { get; set; }
}
