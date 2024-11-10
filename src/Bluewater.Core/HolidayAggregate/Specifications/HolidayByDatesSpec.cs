using Ardalis.Specification;

namespace Bluewater.Core.HolidayAggregate.Specifications;
public class HolidayByDatesSpec : Specification<Holiday>
{
  public HolidayByDatesSpec(DateOnly start, DateOnly end)
  {
    Query.Where(i => 
    DateOnly.FromDateTime(i.Date) >= start && 
    DateOnly.FromDateTime(i.Date) <= end);
  }
}
