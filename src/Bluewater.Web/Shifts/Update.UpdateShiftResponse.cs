namespace Bluewater.Web.Shifts;

public class UpdateShiftResponse(ShiftRecord shift)
{
  public ShiftRecord Shift { get; set; } = shift;
}
