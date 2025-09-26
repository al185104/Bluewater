namespace Bluewater.Web.Leaves;

public class UpdateLeaveResponse(LeaveRecord Leave)
{
  public LeaveRecord Leave { get; set; } = Leave;
}
