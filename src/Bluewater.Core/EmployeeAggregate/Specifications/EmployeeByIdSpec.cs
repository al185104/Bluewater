using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeByIdSpec : Specification<Employee>
{
  public EmployeeByIdSpec(Guid EmployeeId)
  {
    Query
        .Where(Employee => Employee.Id == EmployeeId && !Employee.IsDeleted)
        .Include(Employee => Employee.User)
        .Include(Employee => Employee.Pay)
        .Include(Employee => Employee.Type)
        .Include(Employee => Employee.Level)
        .Include(Employee => Employee.Charging)
            .ThenInclude(charging => charging!.Department)
        .Include(Employee => Employee.Position)
            .ThenInclude(position => position!.Section)
                .ThenInclude(section => section!.Department)
                    .ThenInclude(department => department!.Division);
  }
}
