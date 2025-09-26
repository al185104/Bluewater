namespace Bluewater.Web.Holidays;

public class GetHolidayByIdRequest
{
  public const string Route = "/Holidays/{HolidayId:guid}";
  public static string BuildRoute(Guid holidayId) => Route.Replace("{HolidayId:guid}", holidayId.ToString());

  public Guid HolidayId { get; set; }
}
