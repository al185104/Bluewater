using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.UseCases.EmployeeTypes.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateEmployeeTypeHandler(ICreateEmployeeTypeService _createEmployeeTypeService) : ICommandHandler<CreateEmployeeTypeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateEmployeeTypeCommand request, CancellationToken cancellationToken)
  {
    return await _createEmployeeTypeService.CreateEmployeeTypeAsync(request.Name, request.Value, request.IsActive, cancellationToken);
  }
}
