namespace Bluewater.Web.Leaves;

public class DeleteLeaveRequest
{
  public const string Route = "/Leaves/{LeaveId:guid}";
  public static string BuildRoute(Guid leaveId) => Route.Replace("{LeaveId:guid}", leaveId.ToString());

  public Guid LeaveId { get; set; }
}
