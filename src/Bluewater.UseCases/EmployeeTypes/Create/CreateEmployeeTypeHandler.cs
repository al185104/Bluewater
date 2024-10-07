using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.UseCases.EmployeeTypes.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateEmployeeTypeHandler(IRepository<EmployeeType> _repository) : ICommandHandler<CreateEmployeeTypeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateEmployeeTypeCommand request, CancellationToken cancellationToken)
  {
    var newEmployeeType = new EmployeeType(request.Name, request.Value, request.IsActive);
    var createdItem = await _repository.AddAsync(newEmployeeType, cancellationToken);
    return createdItem.Id;
  }
}
