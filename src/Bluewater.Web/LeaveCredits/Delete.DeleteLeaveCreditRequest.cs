namespace Bluewater.Web.LeaveCredits;

public class DeleteLeaveCreditRequest
{
  public const string Route = "/LeaveCredits/{LeaveCreditId:guid}";
  public static string BuildRoute(Guid leaveCreditId) => Route.Replace("{LeaveCreditId:guid}", leaveCreditId.ToString());

  public Guid LeaveCreditId { get; set; }
}
