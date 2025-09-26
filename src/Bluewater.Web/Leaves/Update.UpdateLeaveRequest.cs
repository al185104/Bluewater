using System.ComponentModel.DataAnnotations;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.Web.Leaves;

public class UpdateLeaveRequest
{
  public const string Route = "/Leaves/{LeaveId:guid}";
  public static string BuildRoute(Guid leaveId) => Route.Replace("{LeaveId:guid}", leaveId.ToString());

  public Guid LeaveId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public DateTime StartDate { get; set; }

  [Required]
  public DateTime EndDate { get; set; }

  public bool IsHalfDay { get; set; }

  [Required]
  public ApplicationStatusDTO Status { get; set; } = ApplicationStatusDTO.NotSet;

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public Guid LeaveCreditId { get; set; }
}
