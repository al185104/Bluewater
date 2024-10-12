using Ardalis.SharedKernel;

namespace Bluewater.Core.HolidayAggregate;
public class Holiday(string name, string? description, DateTime date, bool isRegular) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;
  public DateTime Date { get; private set; } = date;
  public bool IsRegular { get; private set; } = isRegular;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  public Holiday() : this(string.Empty, string.Empty, DateTime.Now, false) { }

  public void UpdateHoliday(string name, string? description, DateTime date, bool isRegular)
  {
    Name = name;
    Description = description;
    Date = date;
    IsRegular = isRegular;
  }
}
