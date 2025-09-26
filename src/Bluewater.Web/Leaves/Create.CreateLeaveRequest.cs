using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Leaves;

public class CreateLeaveRequest
{
  public const string Route = "/Leaves";

  [Required]
  public DateTime? StartDate { get; set; }

  [Required]
  public DateTime? EndDate { get; set; }

  public bool IsHalfDay { get; set; }

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public Guid LeaveCreditId { get; set; }
}
