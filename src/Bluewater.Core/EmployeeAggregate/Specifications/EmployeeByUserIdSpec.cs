using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeByUserIdSpec : Specification<Employee>
{
  public EmployeeByUserIdSpec(Guid userId)
  {
    Query
      .Where(employee => employee.UserId == userId && !employee.IsDeleted)
      .Include(employee => employee.User)
      .Include(employee => employee.Pay)
      .Include(employee => employee.Type)
      .Include(employee => employee.Level)
      .Include(employee => employee.Charging)
        .ThenInclude(charging => charging!.Department)
      .Include(employee => employee.Position)
        .ThenInclude(position => position!.Section)
          .ThenInclude(section => section!.Department)
            .ThenInclude(department => department!.Division);
  }
}
