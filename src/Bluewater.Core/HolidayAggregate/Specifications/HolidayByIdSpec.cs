using Ardalis.Specification;

namespace Bluewater.Core.HolidayAggregate.Specifications;
public class HolidayByIdSpec : Specification<Holiday>
{
  public HolidayByIdSpec(Guid HolidayId)
  {
    Query
        .Where(Holiday => Holiday.Id == HolidayId);
  }
}
