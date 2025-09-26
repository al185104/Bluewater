using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Attendances;

public class AttendanceListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public DateOnly StartDate { get; set; }

  [Required]
  public DateOnly EndDate { get; set; }
}
