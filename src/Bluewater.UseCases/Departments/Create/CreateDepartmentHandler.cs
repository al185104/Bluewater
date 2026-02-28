using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DepartmentAggregate.Specifications;
using Bluewater.UseCases.Departments.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateDepartmentHandler(IRepository<Department> _repository) : ICommandHandler<CreateDepartmentCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
  {
    var existingDepartment = await _repository.FirstOrDefaultAsync(new DepartmentByNameSpec(request.Name), cancellationToken);
    if (existingDepartment != null)
    {
      return existingDepartment.Id;
    }

    var newDepartment = new Department(request.Name, request.Description, request.DivisionId);
    var createdItem = await _repository.AddAsync(newDepartment, cancellationToken);
    return createdItem.Id;
  }
}
