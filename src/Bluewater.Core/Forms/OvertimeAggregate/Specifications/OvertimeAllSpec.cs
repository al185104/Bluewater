using Ardalis.Specification;
using Bluewater.Core.Forms.OvertimeAggregate;

namespace Bluewater.Core.OvertimeAggregate.Specifications;
public class OvertimeAllSpec : Specification<Overtime>
{
  public OvertimeAllSpec()
  {
    Query.Include(Overtime => Overtime.Employee);
  }
}
