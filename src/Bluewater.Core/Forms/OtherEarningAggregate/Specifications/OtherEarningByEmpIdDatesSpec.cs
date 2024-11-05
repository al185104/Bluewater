using Ardalis.Specification;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.Core.OtherEarningAggregate.Specifications;
public class OtherEarningByEmpIdDatesSpec : Specification<OtherEarning>
{
  public OtherEarningByEmpIdDatesSpec(Guid empId, DateOnly start, DateOnly end)
  {
    Query.Where(OtherEarning => OtherEarning.EmployeeId == empId && OtherEarning.Date >= start && OtherEarning.Date <= end);
    Query.Include(OtherEarning => OtherEarning.Employee);
  }
}
