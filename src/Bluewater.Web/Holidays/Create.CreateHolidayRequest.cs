using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Holidays;

public class CreateHolidayRequest
{
  public const string Route = "/Holidays";

  [Required]
  public string? Name { get; set; }

  public string? Description { get; set; }

  [Required]
  public DateTime Date { get; set; }

  public bool IsRegular { get; set; }
}
