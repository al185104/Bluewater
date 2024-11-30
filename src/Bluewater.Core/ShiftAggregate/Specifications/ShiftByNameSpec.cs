using Ardalis.Specification;
using Bluewater.Core.ShiftAggregate;

namespace Bluewater.Core.TimesheetAggregate.Specifications;
public class ShiftByNameSpec : Specification<Shift>
{
  public ShiftByNameSpec(string name)
  {
    Query
        .Where(Shift => Shift.Name.ToLower() == name.ToLower());
  }
}
