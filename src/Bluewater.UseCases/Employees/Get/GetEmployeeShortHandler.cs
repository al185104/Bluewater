using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;

namespace Bluewater.UseCases.Employees.Get;

public class GetEmployeeShortHandler(IReadRepository<Employee> _repository) : IQueryHandler<GetEmployeeShortQuery, Result<EmployeeShortDTO>>
{
  public async Task<Result<EmployeeShortDTO>> Handle(GetEmployeeShortQuery request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeShortByNameSpec(request.EmployeeName);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new EmployeeShortDTO(entity.Id, $"{entity.LastName}, {entity.FirstName}");
  }
}
