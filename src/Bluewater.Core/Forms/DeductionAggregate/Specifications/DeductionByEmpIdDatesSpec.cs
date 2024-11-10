using Ardalis.Specification;
using Bluewater.Core.Forms.DeductionAggregate;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.DeductionAggregate.Specifications;
public class DeductionByEmpIdDatesSpec : Specification<Deduction>
{
  public DeductionByEmpIdDatesSpec(Guid? empId, ApplicationStatus? status, DateOnly end)
  {
    Query.Where(d => 
            (empId == null || d.EmployeeId == empId) &&
            (status == null || d.Status == status) &&
            d.EndDate >= end &&
            d.StartDate < end &&
            d.MonthlyAmortization > 0)
        .Include(e => e.Employee);
  }
}
