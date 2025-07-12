using Ardalis.Specification;

namespace Bluewater.Core.MealCreditAggregate.Specifications;
public class MealCreditsByDateSpec : Specification<MealCredit>
{
  public MealCreditsByDateSpec(List<Guid> empIdList, DateOnly start, DateOnly end)
  {
    Query
        .AsNoTracking()
        .Where(i => i.Date >= start && i.Date <= end && i.EmployeeId != null && empIdList.Contains(i.EmployeeId ?? Guid.Empty));
  }
}
