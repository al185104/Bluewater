namespace Bluewater.Web.Leaves;

public class CreateLeaveResponse(LeaveRecord Leave)
{
  public LeaveRecord Leave { get; set; } = Leave;
}
