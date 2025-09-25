namespace Bluewater.Web.Shifts;

public class GetShiftByIdRequest
{
  public const string Route = "/Shifts/{ShiftId:guid}";
  public static string BuildRoute(Guid shiftId) => Route.Replace("{ShiftId:guid}", shiftId.ToString());

  public Guid ShiftId { get; set; }
}
