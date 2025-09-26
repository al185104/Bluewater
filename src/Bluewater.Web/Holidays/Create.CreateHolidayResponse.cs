namespace Bluewater.Web.Holidays;

public class CreateHolidayResponse(Guid Id, string Name, string? Description, DateTime Date, bool IsRegular)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string? Description { get; set; } = Description;
  public DateTime Date { get; set; } = Date;
  public bool IsRegular { get; set; } = IsRegular;
}
