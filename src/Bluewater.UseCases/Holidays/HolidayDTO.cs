namespace Bluewater.UseCases.Holidays;
public record HolidayDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public DateTime Date { get; set; }
  public bool IsRegular { get; set; }

  public HolidayDTO(Guid id, string name, string? description, DateTime date, bool isRegular) : this()
  {
    Id = id;
    Name = name;
    Description = description;
    Date = date;
    IsRegular = isRegular;
  }
}
