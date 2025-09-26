namespace Bluewater.Web.Holidays;

public class UpdateHolidayResponse(HolidayRecord Holiday)
{
  public HolidayRecord Holiday { get; set; } = Holiday;
}
