using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Holidays;

public class UpdateHolidayRequest
{
  public const string Route = "/Holidays/{HolidayId:guid}";
  public static string BuildRoute(Guid holidayId) => Route.Replace("{HolidayId:guid}", holidayId.ToString());

  public Guid HolidayId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }

  public string? Description { get; set; }

  [Required]
  public DateTime Date { get; set; }

  public bool IsRegular { get; set; }
}
