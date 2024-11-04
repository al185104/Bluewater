using Ardalis.Specification;
using Bluewater.Core.Forms.OvertimeAggregate;

namespace Bluewater.Core.OvertimeAggregate.Specifications;
public class OvertimeByDatesSpec : Specification<Overtime>
{
  public OvertimeByDatesSpec(DateOnly start, DateOnly end)
  {
    Query.Where(Overtime => Overtime.StartDate.HasValue && Overtime.EndDate.HasValue && 
        DateOnly.FromDateTime(Overtime.StartDate.Value) >= start && DateOnly.FromDateTime(Overtime.EndDate.Value) <= end);
    Query.Include(Overtime => Overtime.Employee);
  }
}
